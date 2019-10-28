using Server.General;
using System;
using System.Collections.Generic;
using WatsonWebsocket;
using Models;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using Server.Communication;

namespace Server
{
    class Program
    {
        private readonly static Configurations.ServerConfiguration serverConfig = new Configurations.ServerConfiguration();
        private static WatsonWsServer server;
        private static List<User> clients;

        static void Main(string[] args)
        {
            int exitCode = 0;
            Log.Start();

            try
            {
                CheckConfig();

                Log.Add("Starting server");

                serverConfig.Load();

                clients = new List<User>();
                InitializeModules();

                InitializeServer();
                server.Start();

                while (UserInput.InputString("Type '.stop' to stop the server", "") != ".stop") { Log.Add("Invalid Input", MessageType.Debug); }

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
                catch { /* in case server has not been initialized yet */}

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

        private static void CheckConfig()
        {
            Log.Add("Checking Configurations");
            Configurations.DatabaseConfiguration dbConf = new Configurations.DatabaseConfiguration();

            if (!serverConfig.IsAvailable())
            {
                Log.Add("Server configuration not present. Initiating server configuration dialogue.");
                serverConfig.Setup();
            }

            if (!dbConf.IsAvailable())
            {
                Log.Add("Database configuration not present. Initiating database configuration dialogue");
                dbConf.Setup();
            }

        }

        private static void InitializeModules()
        {
            Log.Add("Initializing Modules");

            RequestHandler.Modules.Add(new Modules.LoginModule());

            foreach (Modules.Module m in RequestHandler.Modules) { Log.Add("Initialized " + m.Name); }
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
                if (!(data != null && data.Length > 0)) throw new ArgumentNullException("Data is empty.");

                Response response = new RequestHandler().HandleRequest(data);

                foreach (User user in response.Header.Targets)
                {
                    await server.SendAsync(user.IpPort, Converter.ConvertStringToBytes(Converter.ConvertObjectToJson(response)));
                }
            }
            catch (Exception ex)
            {
                Log.Add($"Message of client {ipPort} could not be handled: {Environment.NewLine} {ex.ToString()}");
            }

        }

    }
}