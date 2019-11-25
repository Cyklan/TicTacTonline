using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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
        /// <summary>
        /// Der verbundene User und ob eine Anfrage bearbeitet wird
        /// </summary>
        private static Dictionary<User, bool> clients;
        private static readonly Log log = new Log();
        private static bool stopReceive = false;
        private static readonly Converter converter = new Converter();
        private static RequestHandler requestHandler = new RequestHandler();
        private static Cleaner cleaner = new Cleaner();

        static void Main(string[] args)
        {
            int exitCode = 0;
            log.Start();

            try
            {
                CheckConfig();

                log.Add("Starting server");

                serverConfig.Load();
                clients = new Dictionary<User, bool>();

                InitializeModules();
                InitializeServer();

                cleaner.Start();
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
                log.Add("Receiving stopped");
                stopReceive = true;

                if (!(server is null))
                {
                    log.Add("Waiting for requests to be processed");
                    while (clients.Any(x => x.Value)) { Thread.Sleep(10); }

                    log.Add("Disconnecting users");
                    foreach (KeyValuePair<User, bool> user in clients)
                    {
                        server.DisconnectClient(user.Key.IpPort);
                    }

                    server.Dispose();
                }

                cleaner.Stop();

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
                    foreach (KeyValuePair<User, bool> user in clients)
                    {
                        log.Add(user.ToString(), MessageType.Debug);
                    }
                    break;

                case ".pause":
                    stopReceive = true;
                    break;

                case ".continue":
                    stopReceive = false;
                    break;

                case ".clean":
                    cleaner.Clean();
                    break;

                case ".cleanall":
                    cleaner.CleanAll();
                    break;

                default:
                    log.Add($"Commands: {Environment.NewLine}" +
                        $" .stop {Environment.NewLine}" +
                        $" .clients {Environment.NewLine}" +
                        $" .pause {Environment.NewLine}" +
                        $" .continue {Environment.NewLine}" +
                        $" .clean {Environment.NewLine}" +
                        $" .cleanall", MessageType.Debug);
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

        private static void InitializeModules()
        {
            log.Add("Initializing Modules");

            requestHandler.Modules.Add(new LoginModule());
            requestHandler.Modules.Add(new GameModule());
            requestHandler.Modules.Add(new RoomModule());

            foreach (Module m in requestHandler.Modules) { log.Add("Initialized " + m.Name); }
        }

        private static void CheckConfig()
        {
            log.Add("Checking Configurations");
            DatabaseConfiguration dbConf = new DatabaseConfiguration();
            CleanerConfiguration cleanerConf = new CleanerConfiguration();

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

            if (!cleanerConf.IsAvailable())
            {
                log.Add("Cleaner configuration not present. Applying default settings.");
                cleanerConf.Setup();
            }

        }

        public static bool IsUserConnected(string ipPort)
        {
            return clients.Any(x => x.Key.IpPort == ipPort);
        }

        // supresses warning because of missing await
#pragma warning disable CS1998

        private static async Task<bool> ClientConnected(string ipPort, HttpListenerRequest request)
        {
            try
            {
                clients.Add(new User(ipPort), false);
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
                clients.Remove(clients.First(x => x.Key.IpPort == ipPort).Key);
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
                if (stopReceive) { throw new Exception("Handling of requests stopped"); }

                byte[] response = requestHandler.HandleRequest(data, out List<User> targets, clients.FirstOrDefault(x => x.Key.IpPort == ipPort).Key);
                clients[clients.FirstOrDefault(x => x.Key.IpPort == ipPort).Key] = true;

                foreach (User user in targets)
                {
                    User temp = user;
                    if (string.IsNullOrEmpty(user.IpAddress) || user.Port == 0) temp = clients.FirstOrDefault(x => x.Key.Name.ToLower() == user.Name.ToLower()).Key;

                    if (temp == null || !await server.SendAsync(temp.IpPort, response)) log.Add($"Message could not be send to {temp}.", MessageType.Warning);
                }

                clients[clients.FirstOrDefault(x => x.Key.IpPort == ipPort).Key] = false;
            }
            catch (Exception ex)
            {
                log.Add($"Message of client {ipPort} could not be handled: {Environment.NewLine} {ex.ToString()}", MessageType.Error);
            }

        }

  
    }
}