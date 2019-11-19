﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Server.Database;
using Models;

namespace Server.General
{
    public class Cleaner
    {
        private Thread cleanThread;
        private bool run;
        private readonly Log log = new Log();
        private User cleanerUser;
        private readonly Configurations.CleanerConfiguration CleanerConfiguration = new Configurations.CleanerConfiguration();
        private DatabaseQueries db;

        public void Start()
        {
            CleanerConfiguration.Load();

            cleanerUser = new User()
            {
                Name = "Cleaner",
                IpAddress = "0.0.0.0",
                Port = 0
            };

            Log("Starting cleaner");
            cleanThread = new Thread(HandleCleanThread);
            run = true;
            db = new DatabaseQueries(cleanerUser);
            cleanThread.Start();
        }

        public void Stop()
        {
            Log("Stopping cleaner");
            run = false;

            while (cleanThread.IsAlive) { Thread.Sleep(10); }
        }

        private void HandleCleanThread()
        {
            DateTime startTime = new DateTime();

            while (run)
            {
                if (DateTime.Now > startTime.AddMinutes(CleanerConfiguration.IntervalInMinutes))
                {
                    Log("Cleaning started");
                    startTime = DateTime.Now;
                    Clean();
                }
            }
        }

        public void Clean()
        {
            CleanUsers();
            CleanRooms();
        }

        private void CleanUsers()
        {
            Log("Cleaning users started");
            foreach (User user in db.GetUsers())
            {
                if (!db.IsUserLoggedIn(user)) { continue; }
                if (Program.IsUserConnected(user.IpPort)) { continue; }

                db.LogoutUser(user);
                Log($"Cleaned user {user}");
            }
        }

        private void CleanRooms()
        {
            Log("Cleaning rooms started");
            foreach (RoomDocument room in db.GetRooms())
            {
                switch (room.RoomStatus)
                {
                    case RoomStatus.Open:
                        if (room.Game.Player1 is null && room.Game.Player2 is null)
                        {
                            db.ChangeRoomStatus(room.Id, RoomStatus.Closed);
                            break;
                        }
                        continue;

                    default:
                        if (room.Game.Player1 is null || room.Game.Player2 is null)
                        {
                            db.ChangeRoomStatus(room.Id, RoomStatus.Closed);
                            db.RemoveUserFromRoom(room.Game.Player1 is null ? room.Game.Player2 : room.Game.Player1);
                            break;
                        }
                        continue;
    
                }
                Log($"Cleaned room {room.Id}");
            }

            foreach(int room in db.GetRoomsWithoutPlayers())
            {
                db.ChangeRoomStatus(room, RoomStatus.Closed);
                Log($"Cleaned room {room}");
            }
        }

        private void Log(string text)
        {
            log.Add(text, cleanerUser, MessageType.Debug);
        }

    }

}