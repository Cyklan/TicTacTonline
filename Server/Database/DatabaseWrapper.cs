using System;
using System.Collections.Generic;
using Models;
using MySql.Data.MySqlClient;
using Server.Configurations;
using Server.General;

namespace Server.Database
{
    public class DatabaseWrapper
    {
        private readonly MySqlConnection connection;
        private readonly DatabaseConfiguration configuration = new DatabaseConfiguration();
        private readonly User currentUser;

        public DatabaseWrapper(User user)
        {
            configuration.Load();
            connection = new MySqlConnection($"Server={configuration.Ip}; Port={configuration.Port}; Database={configuration.Database}; Uid={configuration.User}; Pwd={configuration.Password};");
            currentUser = user;
            Log.Add("Created MySql-Connection", currentUser, MessageType.Normal);
        }

        public int ExecuteNonQuery(string statement, params string[] parameters)
        {
            try
            {
                connection.Open();
                return CreateCommand(statement, parameters).ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<Dictionary<string, object>> ExecuteQuery(string statement, params string[] parameters)
        {
            try
            {
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;

                connection.Open();
                MySqlDataReader reader = CreateCommand(statement, parameters).ExecuteReader();

                while (reader.Read())
                {
                    row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++) { row.Add(reader.GetName(i), reader.GetValue(i)); }
                    result.Add(row);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        private MySqlCommand CreateCommand(string statement, params string[] parameters)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = statement;
            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue($"@{i}", parameters[i]);
            }

            Log.Add($"Executing command: {statement}", currentUser, MessageType.Normal);
            return command;
        }
    }
}
