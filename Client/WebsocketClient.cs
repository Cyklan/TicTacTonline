using Models;
using System;
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

        public void Initialize()
        {
            if (!WebsocketConfiguration.IsAvailable()) WebsocketConfiguration.Setup();
            WebsocketConfiguration.Load();

            if (client != null) client.Dispose();
            client = new WatsonWsClient(WebsocketConfiguration.GetWebsocketUri());
            client.ServerConnected += onConnect;
            client.ServerDisconnected += onDisconnect;
            client.MessageReceived += onReceive;
            client.Start();
        }

        public Response Exchange(Request request)
        {
            DateTime startWaitTime = DateTime.Now;
            responseToWaitFor = new Response();
            responseToWaitFor.Header = new ResponseHeader() { MessageNumber = request.Header.MessageNumber };

            client.SendAsync(converter.ConvertStringToBytes(converter.ConvertObjectToJson(request)));

            while(responseToWaitFor.Header.Targets is null) { Thread.Sleep(100); }

            return responseToWaitFor;
        }

        public void Send(Request request)
        {
            client.SendAsync(converter.ConvertStringToBytes(converter.ConvertObjectToJson(request)));
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