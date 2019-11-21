using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Models;

namespace Server.General
{
    /// <summary>
    /// Gibt alle Geschehnisse in der Konsole aus.
    /// Wird auf einem extra thread ausgeführt, um andere Arbeitsprozesse nicht zu pausieren
    /// </summary>
    public class Log
    {

        private static Queue<Message> logQueue;
        private static Thread logThread;
        private static bool run;
        private static readonly Pathmanager pathmanager = new Pathmanager();

        /// <summary>
        /// Startet den Log Thread
        /// </summary>
        public void Start()
        {
            logQueue = new Queue<Message>();
            logThread = new Thread(HandleLogQueue);
            run = true;
            logThread.Start();
            Add("Log started");
        }

        /// <summary>
        /// Stoppt den Log Thread
        /// </summary>
        public void Stop()
        {
            Add("Stopping log");
            run = false;
            // logThread.Abort(); - not supported by .net core

            while (logThread.IsAlive) { Thread.Sleep(100); } // wait till thread finished
        }

        /// <summary>
        /// Fügt eine Nachricht ans Ende der Log Liste
        /// Zusätzlich wird der Nutzer, von dem die Nachricht stammt, und die Art der Nachricht, angegeben
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <param name="messageType"></param>
        public void Add(string message, User user, MessageType messageType)
        {
            logQueue.Enqueue(new Message(DateTime.Now, messageType, user, message));
        }

        /// <summary>
        /// Fügt eine Nachricht ans Ende der Log Liste
        /// Zusätzlich wird die Art der Nachricht angegeben
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        public void Add(string message, MessageType messageType)
        {
            logQueue.Enqueue(new Message(DateTime.Now, messageType, new User { Name = "Global" }, message));
        }

        /// <summary>
        /// Fügt eine Nachricht ans Ende der Log Liste
        /// </summary>
        /// <param name="message"></param>
        public void Add(string message)
        {
            logQueue.Enqueue(new Message(DateTime.Now, MessageType.Normal, new User { Name = "Global" }, message));
        }

        /// <summary>
        /// Gibt alle Elemente in der Log Liste nacheinander aus
        /// </summary>
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

                    // Die Textfarbe der Konsole wird nicht zurückgesetzt, weil diese *immer* für jede Nachricht neu gesetzt wird.
                }

                Thread.Sleep(10);
            }

        }
    }
}
