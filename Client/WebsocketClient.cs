using Models;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace Client
{
    public class WebsocketClient
    {
        private WatsonWsClient client;
        private WebsocketConfiguration WebsocketConfiguration = new WebsocketConfiguration();
        private Converter converter = new Converter();
        private Response responseToWaitFor;


        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnSpontaneousReceive;

        public bool Initialize()
        {
            if (!WebsocketConfiguration.IsAvailable()) WebsocketConfiguration.Setup();
            WebsocketConfiguration.Load();

            if (!PingServer()) return false;

            if (client != null) client.Dispose();
            client = new WatsonWsClient(WebsocketConfiguration.GetWebsocketUri());
            client.ServerConnected += onConnect;
            client.ServerDisconnected += onDisconnect;
            client.MessageReceived += onReceive;
            client.Start();

            return true;
        }

        public Response Exchange(Request request)
        {
            DateTime startWaitTime = DateTime.Now;
            responseToWaitFor = new Response();
            responseToWaitFor.Header = new ResponseHeader() { MessageNumber = request.Header.MessageNumber };

            client.SendAsync(converter.ConvertStringToBytes(converter.ConvertObjectToJson(request)));

            while (responseToWaitFor.Header.Targets is null) { Thread.Sleep(100); }

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
                using TcpClient client = new TcpClient(WebsocketConfiguration.Ip, WebsocketConfiguration.Port);
                return true;
            }
            catch
            {
                return false;
            }
        }

#pragma warning disable CS1998

        private async Task onConnect()
        {
            OnConnect?.Invoke(this, new EventArgs());
        }

        private async Task onDisconnect()
        {
            OnDisconnect?.Invoke(this, new EventArgs());
        }

        private async Task onReceive(byte[] data)
        {
            Response response = converter.ConvertJsonToObject<Response>(converter.ConvertBytesToString(data));

            if (responseToWaitFor.Header.MessageNumber == response.Header.MessageNumber)
            {
                responseToWaitFor = response;
                return;
            }

            OnSpontaneousReceive?.Invoke(response, new EventArgs());
        }

#pragma warning restore CS1998
    }
}