﻿using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace Server.Database
{

    public class DatabaseQueries : IDisposable
    {
        private DatabaseWrapper database;

        public DatabaseQueries(User user)
        {
            database = new DatabaseWrapper(user);
        }

        public List<User> GetUsers()
        {
            List<User> result = new List<User>();

            foreach (Dictionary<string, object> row in database.ExecuteQuery("SELECT * FROM users;"))
            {
                result.Add(new User { Name = row["name"].ToString() });
            }

            return result;
        }

        public bool LoginUser(User user)
        {
            if (database.ExecuteQuery($@"SELECT * FROM users WHERE hash='{user.PasswordHash}' AND name='{user.Name}';").Count != 1) return false;

            database.ExecuteNonQuery($@"UPDATE users SET loggedin=1 WHERE name='{user.Name}';");

            return true;
        }

        public bool LogoutUser(User user) => database.ExecuteNonQuery($@"UPDATE users SET loggedin=0 WHERE name='{user.Name}';") == 1;

        public int RegisterUser(User user) => database.ExecuteNonQuery($"INSERT INTO users(name, hash) VALUES ('{user.Name}', '{user.PasswordHash}');");

        public bool IsUserLoggedIn(User user) => database.ExecuteQuery($"SELECT * FROM users WHERE name='{user.Name}' AND loggedin=1;").Count == 1;

        public int CreateNewGame(string name, string password) {
            database.ExecuteNonQuery($"INSERT INTO rooms (name, statusid, password) VALUES('{name}', {Modules.RoomStaus.Open}, '{password}');");
            List<Dictionary<string, object>> rooms = database.ExecuteQuery($"SELECT * FROM rooms WHERE name='{name}' AND password='{password}';");
           
            if (rooms.Any()) return (int)rooms.Last()["id"];
            return -1;
        }

        public bool AddUserToGame(User user, int roomId) => database.ExecuteNonQuery($"UPDATE users SET roomid = '{roomId}' WHERE (name='{user.Name}');") == 1;

        public void Dispose()
        {
            database = null;
        }
    }
}
