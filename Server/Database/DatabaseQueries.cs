using System;
using System.Collections.Generic;
using Models;

namespace Server.Database
{
    public class DatabaseQueries : IDisposable
    {
        private DatabaseWrapper database = new DatabaseWrapper();

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
            if (database.ExecuteQuery("SELECT * FROM users WHERE hash='@0' and name='@1';", user.PasswordHash, user.Name).Count != 1) return false;

            database.ExecuteNonQuery("UPDATE users SET loggedin=1 WHERE name='@1';");

            return true;
        }

        public int RegisterUser(User user) => database.ExecuteNonQuery("INSERT INTO users(name, hash) VALUES ('@1', '@2');", user.Name, user.PasswordHash);

        public void Dispose()
        {
            database = null;
        }
    }
}
