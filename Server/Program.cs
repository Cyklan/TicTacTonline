using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Models;
using Server.Communication;
using Server.Configurations;
using Server.General;
using Server.Modules;
using WatsonWebsocket;

namespace Server
{
    class Program
    {
        private static readonly ServerConfiguration serverConfig = new ServerConfiguration();
        private static WatsonWsServer server;
        private static List<User> clients;
        private static readonly Log log = new Log();
        private static bool stopReceive = false;
        private static readonly Converter converter = new Converter();

        static void Main(string[] args)
        {
            int exitCode = 0;
            log.Start();

            try
            {
                CheckConfig();

                log.Add("Starting server");

                serverConfig.Load();

                clients = new List<User>();
                InitializeModules();

                InitializeServer();
                server.Start();

                while (ShowAndHandleMenu(Console.ReadLine())) { }

            }
            catch (Exception ex)
            {
                exitCode = 1;
                log.Add(ex.ToString(), MessageType.Error);
            }
            finally
            {
                log.Add("Closing server");

                if (!(server is null)) { clients.ForEach(x => server.DisconnectClient(x.IpPort)); server.Dispose(); }
                log.Stop();
                GC.Collect();
                Environment.Exit(exitCode);

            }
        }

        private static bool ShowAndHandleMenu(string input)
        {
            switch (input)
            {
                case ".stop":
                    return false;

                case ".clients":
                    clients.ForEach(x => log.Add(x.ToString(), MessageType.Debug));
                    break;

                case ".pause":
                    stopReceive = true;
                    break;

                case ".continue":
                    stopReceive = false;
                    break;
                default:
                    log.Add($"Commands: {Environment.NewLine} .stop {Environment.NewLine} .clients {Environment.NewLine} .pause {Environment.NewLine} .continue", MessageType.Debug);
                    break;
            }

            return true;
        }

        private static void InitializeServer()
        {
            server = new WatsonWsServer(serverConfig.Ip, serverConfig.Port, serverConfig.Ssl);

            server.ClientConnected += ClientConnected;
            server.ClientDisconnected += ClientDisconnected;
            server.MessageReceived += MessageReceived;

            log.Add($"Server running at {serverConfig.Ip}:{serverConfig.Port}");
        }

        private static void CheckConfig()
        {
            log.Add("Checking Configurations");
            DatabaseConfiguration dbConf = new DatabaseConfiguration();

            if (!serverConfig.IsAvailable())
            {
                log.Add("Server configuration not present. Applying default settings.");
                serverConfig.Setup();
            }

            if (!dbConf.IsAvailable())
            {
                log.Add("Database configuration not present. Applying default settings.");
                dbConf.Setup();
            }

        }

        private static void InitializeModules()
        {
            log.Add("Initializing Modules");

            RequestHandler.Modules.Add(new LoginModule());
            RequestHandler.Modules.Add(new GameModule());

            foreach (Module m in RequestHandler.Modules) { log.Add("Initialized " + m.Name); }
        }

        // supresses warning because of missing await
#pragma warning disable CS1998

        private static async Task<bool> ClientConnected(string ipPort, HttpListenerRequest request)
        {
            try
            {
                clients.Add(new User(ipPort));
                log.Add($"Client {ipPort} connected");
            }
            catch (Exception ex)
            {
                log.Add($"Client {ipPort} could not connect: {Environment.NewLine} {ex}", MessageType.Error);
                return false;
            }

            return true;
        }

        private static async Task ClientDisconnected(string ipPort)
        {

            try
            {
                clients.Remove(clients.First(x => x.IpPort == ipPort));
                log.Add($"Client {ipPort} disconnected");
            }
            catch (Exception ex)
            {
                log.Add($"Client {ipPort} could not remove client: {Environment.NewLine} {ex}", MessageType.Error);
            }

        }

#pragma warning restore CS1998

        private static async Task MessageReceived(string ipPort, byte[] data)
        {
            if (stopReceive) return;

            try
            {
                if (!(data != null && data.Length > 0)) throw new ArgumentNullException("Data is empty.");

                Request request = converter.ConvertJsonToObject<Request>(converter.ConvertBytesToString(data));
                SyncClientData(ipPort, request.Header.User);

                log.Add($"Received {converter.ConvertObjectToJson(request)} from {request.Header.User}", request.Header.User, MessageType.Normal);

                Response response = new RequestHandler().HandleRequest(request);
                log.Add($"Sending {converter.ConvertObjectToJson(response)} to {string.Join(",", response.Header.Targets.Select(x => x.ToString()))}", request.Header.User, MessageType.Normal);

                foreach (User user in response.Header.Targets)
                {
                    User temp = user;
                    if (string.IsNullOrEmpty(user.IpAddress) || user.Port == 0) { temp = clients.FirstOrDefault(x => x.Name == user.Name); }

                    if (temp == null || !await server.SendAsync(temp.IpPort, converter.ConvertStringToBytes(converter.ConvertObjectToJson(response))))
                    {
                        log.Add($"Message {response.Header.MessageNumber} could not be send to {temp}.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Add($"Message of client {ipPort} could not be handled: {Environment.NewLine} {ex.ToString()}", MessageType.Error);
            }

        }

        private static void SyncClientData(string ipPortRequest, User requestUser)
        {
            User connectedClient = clients.FirstOrDefault(x => x.IpPort == ipPortRequest);

            connectedClient.Name = requestUser.Name;
            connectedClient.PasswordHash = requestUser.PasswordHash;

            requestUser.IpPort = ipPortRequest;
        }

    }
}