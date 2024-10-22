﻿using Models;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WatsonWebsocket;
using Client.General;
using System.Collections.Generic;

namespace Client.Communication
{
    public class WebsocketClient
    {
        private WatsonWsClient client;
        private readonly WebsocketConfiguration WebsocketConfiguration = new WebsocketConfiguration();
        private readonly Converter converter = new Converter();
        private Response responseToWaitFor;
        private readonly List<string> responsesToIgnore = new List<string>();

        public event EventHandler OnSpontaneousReceive;
        public bool IsConnected
        {
            get
            {
                if (client is null) return false;
                return client.Connected;
            }
        }

        public bool Initialize()
        {
            if (!WebsocketConfiguration.IsAvailable()) WebsocketConfiguration.Setup();
            WebsocketConfiguration.Load();

            if (!PingServer()) return false;

            if (client != null) client.Dispose();
            client = new WatsonWsClient(WebsocketConfiguration.GetWebsocketUri());

            client.MessageReceived += OnReceive;
            client.Start();

            return true;
        }

        public void Close()
        {
            client.Dispose();
        }

        public Response Exchange(Request request)
        {
            DateTime startWaitTime = DateTime.Now;
            responseToWaitFor = new Response
            {
                Header = new ResponseHeader() { MessageNumber = request.Header.MessageNumber }
            };

            client.SendAsync(converter.ConvertStringToBytes(converter.ConvertObjectToJson(request)));

            while (responseToWaitFor.Header.Targets is null)
            {
                if (DateTime.Now > startWaitTime.AddSeconds(15))
                {
                    responsesToIgnore.Add(responseToWaitFor.Header.MessageNumber);
                    responseToWaitFor = null;
                    throw new Exception("Request timed out");
                }
                Thread.Sleep(100);
            }

            if (responseToWaitFor.Header.Code == ResponseCode.UnplannedError) { throw new Exception(responseToWaitFor.Header.Message); }

            return responseToWaitFor;
        }

        public void Send(Request request)
        {
            client.SendAsync(converter.ConvertStringToBytes(converter.ConvertObjectToJson(request)));
        }

        public bool PingServer()
        {
            try
            {
                using (TcpClient client = new TcpClient(WebsocketConfiguration.Ip, WebsocketConfiguration.Port)) { };
                return true;
            }
            catch
            {
                return false;
            }
        }

#pragma warning disable CS1998

        private async Task OnReceive(byte[] data)
        {

            Response response = converter.ConvertJsonToObject<Response>(converter.ConvertBytesToString(data));

            response.Body = GetBody(data);

            if (!(responseToWaitFor is null) && responseToWaitFor.Header.MessageNumber == response.Header.MessageNumber)
            {
                responseToWaitFor = response;
                return;
            }

            if (responsesToIgnore.Contains(response.Header.MessageNumber)) { return; }

            OnSpontaneousReceive?.Invoke(response, new EventArgs());
        }

        private Document GetBody(byte[] data)
        {
            string json = converter.ConvertBytesToString(data);

            string bodyJson = json.Split(new string[] { "\"Body\": " }, StringSplitOptions.None)[1];
            bodyJson = bodyJson.Trim();
            bodyJson = bodyJson.Remove(bodyJson.Length - 1);
            string[] bodySplit = bodyJson.Split(',');

            string type = bodySplit[bodySplit.Length - 1];

            if (type.Contains("RemovePlayerFromRoomDocument"))
            {
                return converter.ConvertJsonToObject<RemovePlayerFromRoomDocument>(bodyJson);
            }
            else if (type.Contains("RoomDocument"))
            {
                return converter.ConvertJsonToObject<RoomDocument>(bodyJson);
            }
            else if (type.Contains("RoomsDocument"))
            {
                return converter.ConvertJsonToObject<RoomsDocument>(bodyJson);
            }
            else if (type.Contains("ChatDocument"))
            {
                return converter.ConvertJsonToObject<ChatDocument>(bodyJson);
            }
            else if (type.Contains("MatchHistoryDocument"))
            {
                return converter.ConvertJsonToObject<MatchHistoryDocument>(bodyJson);
            }
            else if (type.Contains("LeaderboardDocument"))
            {
                return converter.ConvertJsonToObject<LeaderboardDocument>(bodyJson);
            }

            return new Document();

        }

#pragma warning restore CS1998
    }
}