using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace Server.General
{
    public static class Log
    {

        private static Queue<Message> logQueue;
        private static System.Threading.Thread logThread;
        private static bool run;

        public static void Start()
        {
            logQueue = new Queue<Message>();
            logThread = new System.Threading.Thread(new System.Threading.ThreadStart(HandleLogQueue));
            run = true;
            logThread.Start();
            Add("Log started");
        }

        public static void Stop()
        {
            Add("Stopping log");
            run = false;
            // logThread.Abort(); - not supported by .net core

            while (logThread.IsAlive) { System.Threading.Thread.Sleep(100); } // wait till thread finished
        }

        public static void Add(string message, User user, MessageType messageType)
        {
            logQueue.Enqueue(new Message(DateTime.Now, messageType, user, message));
        }

        public static void Add(string message, MessageType messageType)
        {
            logQueue.Enqueue(new Message(DateTime.Now, messageType, new User() { Name = "Global"} , message));
        }

        public static void Add(string message)
        {
            logQueue.Enqueue(new Message(DateTime.Now, MessageType.Normal, new User() { Name = "Global" }, message));
        }

        private static void HandleLogQueue()
        {
            while (run)
            {
                while (logQueue.Any())
                {
                    Message message = logQueue.Dequeue();

                    switch (message.MessageType)
                    {
                        case MessageType.Normal:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case MessageType.Warning:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case MessageType.Error:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case MessageType.Debug:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case MessageType.Question:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                    }


                    System.IO.File.AppendAllText(Pathmanager.LogFilePath, message.ToString() + Environment.NewLine);
                    if (message.MessageType == MessageType.Question)
                    {
                        Console.Write(message);
                    } else
                    {
                        Console.WriteLine(message);
                    }
                }

                System.Threading.Thread.Sleep(10);
            }

        }
    }
}
