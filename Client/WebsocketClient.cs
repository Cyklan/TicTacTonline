﻿using Models;
using System;
using WatsonWebsocket;

namespace Client
{
    public class WebsocketClient
    {
        private WatsonWsClient client;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnSpontaneousReceive;

        public void Initialize()
        {

        }

        public Response Exchange(Request request)
        {
            return new Response();
        }

        public void Send(Request request)
        {

        }

        private void onConnect()
        {

        }

        private void onDisconnect()
        {

        }

        private void onReceive()
        {

        }

    }
}