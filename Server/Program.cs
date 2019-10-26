using Server.General;
using System;
using System.Collections.Generic;
using WatsonWebsocket;
using Models;
using System.Threading.Tasks;
using System.Net;
using System.Linq;

namespace Server
{
    class Program
    {
        private static Configurations.ServerConfiguration serverConfig = new Configurations.ServerConfiguration();
        private static WatsonWsServer server;
        private static List<User> clients;

        static void Main(string[] args)
        {
            int exitCode = 0;
            Log.Start();

            try
            {
                Log.Add("Starting server");

                serverConfig.Load();
                clients = new List<User>();

                InitializeServer();
                server.Start();

                while (Console.ReadLine() != ".stop") { Log.Add("Type '.stop' to stop the server", MessageType.Debug); }

            }
            catch (Exception ex)
            {
                exitCode = 1;
                Log.Add(ex.ToString(), MessageType.Error);
            }
            finally
            {
                Log.Add("Closing server");

                try
                {
                    server.Dispose();
                }
                catch
                { /* in case server has not been initialized yet */}

                Log.Stop();
                GC.Collect();
                Environment.Exit(exitCode);

            }
        }

        private static void InitializeServer()
        {
            server = new WatsonWsServer(serverConfig.Ip, serverConfig.Port, serverConfig.Ssl);

            server.ClientConnected += ClientConnected;
            server.ClientDisconnected += ClientDisconnected;
            server.MessageReceived += MessageReceived;

            Log.Add($"Server running at {serverConfig.Ip}:{serverConfig.Port}");
        }
        // supresses warning because of missing await
#pragma warning disable CS1998

        private static async Task<bool> ClientConnected(string ipPort, HttpListenerRequest request)
        {
            try
            {
                clients.Add(new User(ipPort));
                Log.Add($"Client {ipPort} connected");
            }
            catch (Exception ex)
            {
                Log.Add($"Client {ipPort} could not connect: {Environment.NewLine} {ex.ToString()}", MessageType.Error);
                return false;
            }

            return true;
        }

        private static async Task ClientDisconnected(string ipPort)
        {

            try
            {
                clients.Remove(clients.First(x => x.IpPort == ipPort));
                Log.Add($"Client {ipPort} disconnected");
            }
            catch (Exception ex)
            {
                Log.Add($"Client {ipPort} could not remove client: {Environment.NewLine} {ex.ToString()}", MessageType.Error);
            }

        }

#pragma warning restore CS1998

        private static async Task MessageReceived(string ipPort, byte[] data)
        {
            try
            {
                if (!(data != null && data.Length > 0)) return;

                Communication.CommunicationWrapper communicationWrapper = new Communication.RequestHandler().HandleRequest(data);

                foreach (User user in communicationWrapper.Targets)
                {
                    await server.SendAsync(user.IpPort, communicationWrapper.ResponseData);
                }
            }
            catch
            {
                Log.Add($"Message of client {ipPort} could not be handled");
            }

        }

    }
}