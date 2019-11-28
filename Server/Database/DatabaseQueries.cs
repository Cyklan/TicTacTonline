using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Database
{

    public class DatabaseQueries
    {
        private readonly DatabaseWrapper database;

        public DatabaseQueries(User user)
        {
            database = new DatabaseWrapper(user);
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

                result.Add(new User { Name = row["name"].ToString(), IpAddress = ip, Port = port});
            }

            return result;
        }

        public bool LoginUser(User user)
        {
            if (database.ExecuteQuery($@"SELECT * FROM users WHERE hash='{user.PasswordHash}' AND name='{user.Name}';").Count != 1) return false;

            database.ExecuteNonQuery($@"UPDATE users SET loggedin = TRUE, ip='{user.IpAddress}', port={user.Port} WHERE name='{user.Name}';");

            return true;
        }

        public bool LogoutUser(User user) => database.ExecuteNonQuery($@"UPDATE users SET loggedin = FALSE, ip=NULL, port=NULL, roomid=NULL WHERE name='{user.Name}';") == 1;

        public void LogoutAllUsers() => database.ExecuteNonQuery("UPDATE users SET loggedin = FALSE WHERE loggedin = TRUE;");

        public void CloseAllRooms() => database.ExecuteNonQuery($"UPDATE rooms SET statusid = {(int)RoomStatus.Closed} WHERE statusid != {(int)RoomStatus.Closed};");

        public bool RegisterUser(User user) => database.ExecuteNonQuery($"INSERT INTO users(name, hash) VALUES ('{user.Name}', '{user.PasswordHash}');") == 1;

        public bool IsUserLoggedIn(User user) => database.ExecuteQuery($"SELECT * FROM users WHERE name='{user.Name}' AND loggedin = TRUE;").Count == 1;
        #endregion

        #region Rooms

        public int CreateNewGame(string name, string password)
        {
            database.ExecuteNonQuery($"INSERT INTO rooms (name, statusid, password) VALUES('{name}', {(int)RoomStatus.Open}, '{password}');");
            List<Dictionary<string, object>> rooms = database.ExecuteQuery($"SELECT * FROM rooms WHERE name='{name}' AND password='{password}';");

            if (rooms.Any()) return (int)rooms.Last()["id"];
            return -1;
        }

        public bool AddUserToRoom(User user, int roomId) => database.ExecuteNonQuery($"UPDATE users SET roomid = '{roomId}' WHERE (name='{user.Name}');") == 1;

        public List<int> GetRoomsWithoutPlayers()
        {
            List<int> rooms = new List<int>();

            database.ExecuteQuery($"SELECT id FROM rooms r LEFT JOIN users u ON r.id = u.roomid WHERE statusid !={(int)RoomStatus.Closed} AND u.roomid IS NULL;").ForEach(room =>
            {
                rooms.Add((int)room["id"]);
            });

            return rooms;
        }

        public List<RoomDocument> GetRooms()
        {
            List<RoomDocument> rooms = new List<RoomDocument>();

            database.ExecuteQuery($"SELECT id, r.name as roomname, statusid, password, u.name as username, hash, loggedin, roomid, ip, port FROM rooms r JOIN users u ON r.id=u.roomid WHERE statusid != {(int)RoomStatus.Closed};").ForEach(room =>
            {
                if (!rooms.Any(x => x.Id == (int)room["id"]))
                {
                    rooms.Add(new RoomDocument()
                    {
                        Name = room["roomname"].ToString(),
                        Password = room["password"].ToString(),
                        Id = (int)room["id"],
                        RoomStatus = (RoomStatus)room["statusid"],
                        Game = new Game()
                        {
                            Player1 = new User()
                            {
                                Name = room["username"].ToString(),
                                IpAddress = room["ip"].ToString(),
                                Port = room["port"] is DBNull ? 0 : (int)room["port"]
                            }
                        }
                    });

                }
                else
                {
                    rooms.First(x => x.Id == (int)room["id"]).Game.Player2 = new User()
                    {
                        Name = room["username"].ToString(),
                        IpAddress = room["ip"].ToString(),
                        Port = room["port"] is DBNull ? 0 : (int)room["port"]
                    };
                }

            });

            return rooms;
        }

        public bool SaveMessage(string name, string message, int roomId) => database.ExecuteNonQuery($"INSERT INTO messages (user, content, roomid) VALUES ('{name}', '{message}', {roomId});") == 1;

        public bool RemoveUserFromRoom(User user) => database.ExecuteNonQuery($"UPDATE users SET roomid = NULL WHERE name = '{user.Name}';") == 1;

        public bool ChangeRoomStatus(int roomId, RoomStatus status) => database.ExecuteNonQuery($"UPDATE rooms SET statusid = {(int)status} WHERE id = {roomId};") == 1;

        #endregion

        #region Game

        public void InsertMatch(RoomDocument room, User winner)
        {
            string winnerName = string.IsNullOrEmpty(winner.Name) ? $"{room.Game.Player1.Name}_{room.Game.Player2.Name}" : $"{winner.Name}";
            string sql = "";
            sql += $"INSERT INTO matches (won_by, roomid) VALUES ('{winnerName}', {room.Id});" + Environment.NewLine;
            sql += $"INSERT INTO users_played_matches (users_name, matches_id) VALUES ('{room.Game.Player1.Name}', (SELECT id FROM matches WHERE won_by='{winnerName}' ORDER BY timestamp DESC LIMIT 1));" + Environment.NewLine;
            sql += $"INSERT INTO users_played_matches (users_name, matches_id) VALUES ('{room.Game.Player2.Name}', (SELECT id FROM matches WHERE won_by='{winnerName}' ORDER BY timestamp DESC LIMIT 1));";

            database.ExecuteNonQuery(sql);
        }

        public List<MatchHistoryPosition> GetMatchHistory(User user)
        {
            string sql = "";
            sql += $"SELECT id, won_by, timestamp, roomid, users_name,";
            sql += $"(SELECT users_name FROM users_played_matches WHERE users_played_matches.matches_id = m.id AND users_name NOT LIKE '{user.Name}') AS opponent ";
            sql += "FROM matches m JOIN users_played_matches um ON m.id = um.matches_id ";
            sql += $"WHERE um.users_name LIKE '{user.Name}' LIMIT 20;";

            return database.ExecuteQuery(sql).Select(GetMatchHistoryPosition).ToList();
        }

        private MatchHistoryPosition GetMatchHistoryPosition(Dictionary<string, object> data)
        {
            string winner = data["won_by"].ToString().ToLower();
            bool? won = null;

            if (winner == data["users_name"].ToString().ToLower()) won = true;
            if (winner == data["opponent"].ToString().ToLower()) won = false;

            return new MatchHistoryPosition()
            {
                EnemyUserName = data["opponent"].ToString(),
                IsWin = won,
                TimeStamp = DateTime.Parse(data["timestamp"].ToString())
            };
        }

        #endregion

        #region Elo

        public int GetElo(User user)
        {
            string sql = $"SELECT elo FROM users WHERE name = '{user.Name}'";
            return (int)database.ExecuteQuery(sql)[0]["elo"];
        }

        internal void UpdateElo(User player1, User player2, int player1Elo, int player2Elo)
        {
            //UPDATE `tictactonline`.`users` SET `elo`= '1205' WHERE `name`= '1';
            string sql = $"UPDATE users SET elo = {player1Elo} WHERE name = '{player1.Name}';";
            sql += $"UPDATE users SET elo = {player2Elo} WHERE name = '{player2.Name}';";
            database.ExecuteNonQuery(sql);
        }


        #endregion
    }
}
