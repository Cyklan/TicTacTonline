using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Models;

namespace Server.General
{
    public class Log
    {

        private static Queue<Message> logQueue;
        private static Thread logThread;
        private static bool run;
        private static readonly Pathmanager pathmanager = new Pathmanager();

        public void Start()
        {
            logQueue = new Queue<Message>();
            logThread = new Thread(HandleLogQueue);
            run = true;
            logThread.Start();
            Add("Log started");
        }

        public void Stop()
        {
            Add("Stopping log");
            run = false;
            // logThread.Abort(); - not supported by .net core

            while (logThread.IsAlive) { Thread.Sleep(100); } // wait till thread finished
        }

        public void Add(string message, User user, MessageType messageType)
        {
            logQueue.Enqueue(new Message(DateTime.Now, messageType, user, message));
        }

        public void Add(string message, MessageType messageType)
        {
            logQueue.Enqueue(new Message(DateTime.Now, messageType, new User { Name = "Global" }, message));
        }

        public void Add(string message)
        {
            logQueue.Enqueue(new Message(DateTime.Now, MessageType.Normal, new User { Name = "Global" }, message));
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

                    File.AppendAllText(pathmanager.LogFilePath, message + Environment.NewLine);
                    if (message.MessageType == MessageType.Question)
                    {
                        Console.Write(message);
                    }
                    else
                    {
                        Console.WriteLine(message);
                    }
                }

                Thread.Sleep(10);
            }

        }
    }
}
