using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Database
{

    public class DatabaseQueries : IDisposable
    {
        private DatabaseWrapper database;

        public DatabaseQueries(User user)
        {
            database = new DatabaseWrapper(user);
        }

        public void Dispose()
        {
            database = null;
        }

        #region Login
        public List<User> GetUsers()
        {
            List<User> result = new List<User>();

            foreach (Dictionary<string, object> row in database.ExecuteQuery("SELECT * FROM users;"))
            {
                string ip;
                int port;
                if (row["ip"] is DBNull) { ip = ""; } else { ip = row["ip"].ToString(); }
                if (row["port"] is DBNull) { port = 0; } else { port = (int)row["port"]; }

                result.Add(new User { Name = row["name"].ToString(), IpAddress = ip, Port = port });
            }

            return result;
        }

        public bool LoginUser(User user)
        {
            if (database.ExecuteQuery($@"SELECT * FROM users WHERE hash='{user.PasswordHash}' AND name='{user.Name}';").Count != 1) return false;

            database.ExecuteNonQuery($@"UPDATE users SET loggedin=1, ip='{user.IpAddress}', port={user.Port} WHERE name='{user.Name}';");

            return true;
        }

        public bool LogoutUser(User user) => database.ExecuteNonQuery($@"UPDATE users SET loggedin=0, ip=NULL, port=NULL, roomid=NULL WHERE name='{user.Name}';") == 1;

        public bool RegisterUser(User user) => database.ExecuteNonQuery($"INSERT INTO users(name, hash) VALUES ('{user.Name}', '{user.PasswordHash}');") == 1;

        public bool IsUserLoggedIn(User user) => database.ExecuteQuery($"SELECT * FROM users WHERE name='{user.Name}' AND loggedin=1;").Count == 1;
        #endregion

        #region Rooms

        public int CreateNewGame(string name, string password)
        {
            database.ExecuteNonQuery($"INSERT INTO rooms (name, statusid, password) VALUES('{name}', {RoomStatus.Open}, '{password}');");
            List<Dictionary<string, object>> rooms = database.ExecuteQuery($"SELECT * FROM rooms WHERE name='{name}' AND password='{password}';");

            if (rooms.Any()) return (int)rooms.Last()["id"];
            return -1;
        }

        public bool AddUserToRoom(User user, int roomId) => database.ExecuteNonQuery($"UPDATE users SET roomid = '{roomId}' WHERE (name='{user.Name}');") == 1;

        public List<RoomDocument> GetRooms()
        {
            List<RoomDocument> rooms = new List<RoomDocument>();

            database.ExecuteQuery($"SELECT * FROM rooms r JOIN users u ON r.id=u.roomid WHERE statusid != {RoomStatus.Closed};").ForEach(room =>
            {
                if (!rooms.Any(x => x.Id == (int)room["id"]))
                {
                    rooms.Add(new RoomDocument()
                    {
                        Name = room["name"].ToString(),
                        Password = room["password"].ToString(),
                        Id = (int)room["id"],
                        RoomStatus = (RoomStatus)room["statusid"],
                        Game = new Game()
                        {
                            Player1 = new User()
                            {
                                Name = room["u.name"].ToString(),
                                IpAddress = room["u.ip"].ToString(),
                                Port = (int)room["u.port"]
                            }
                        }
                    });

                }
                else
                {
                    rooms.First(x => x.Id == (int)room["id"]).Game.Player2 = new User()
                    {
                        Name = room["u.name"].ToString(),
                        IpAddress = room["u.ip"].ToString(),
                        Port = (int)room["u.port"]
                    };
                }

            });

            return rooms;
        }

        public bool SaveMessage(string name, string message, int roomId) => database.ExecuteNonQuery($"INSERT INTO messages (user, content, roomid) VALUES ('{name}', '{message}', {roomId});") == 1;

        public bool RemoveUserFromRoom(User user) => database.ExecuteNonQuery($"UPDATE users SET roomid = NULL WHERE name = '{user.Name}';") == 1;

        public bool ChangeRoomStatus(int roomId, RoomStatus status) => database.ExecuteNonQuery($"UPDATE rooms SET status = {status} WHERE id = {roomId};") == 1;

        #endregion

        #region "Game"

        public void InsertMatch(RoomDocument room, User winner)
        {
            string sql = "";
            sql += $"INSERT INTO matches (won_by, roomid) VALUES ('{winner.Name}', {room.Id});" + Environment.NewLine;
            sql += $"INSERT INTO users_played_matches (users_name, matches_id) VALUES ('{room.Game.Player1.Name}', (SELECT id FROM matches WHERE won_by='{winner.Name}' ORDER BY timestamp DESC LIMIT 1));" + Environment.NewLine;
            sql += $"INSERT INTO users_played_matches (users_name, matches_id) VALUES ('{room.Game.Player2.Name}', (SELECT id FROM matches WHERE won_by='{winner.Name}' ORDER BY timestamp DESC LIMIT 1));";

            database.ExecuteNonQuery(sql);
        }

        #endregion
    }
}
