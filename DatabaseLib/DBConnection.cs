using Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace DatabaseLib
{

    //Inspired/borrowed by https://stackoverflow.com/questions/21618015/how-to-connect-to-mysql-database - Ocph23 & Moffen

    public class DBConnection
    {
        private DBConnection()
        {
        }

        public string DatabaseName { get; set; } = "intrusive";
        public string Password { get; set; } = "password";
        public int ServerPort { get; set; } = 3306;
        public string ServerIP { get; set; } = "localhost";
        public string Username { get; set; } = "root";

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public MySqlConnection CreateConnection()
        {
            string connstring = string.Format("datasource={0}; port={1}; database={2}; username={3}; password={4}", ServerIP, ServerPort, DatabaseName, Username, Password);
            MySqlConnection connection = new MySqlConnection(connstring);
            return connection;
        }

        public T Insert<T>(T data) where T : class
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    connection.Insert(data);

                    //connection.Query<AccountModel>("select * from accounts");
                    Console.WriteLine("{0} inserted succesfully!", data.GetType());
                    return data;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return data;
                }
            }
        }
        public List<T> GetAll<T>() where T : class
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    var data = connection.GetAll<T>().ToList();
                    Console.WriteLine("{0} entries recieved!", data.Count());
                    return data;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }
        public T Get<T>(string key) where T : class
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    var data = connection.Get<T>(key);
                    if (data != null)
                        Console.WriteLine("{0} retrieved!", data.GetType());
                    else
                        Console.WriteLine("No object with key: '{0}' was found", key);
                    return data;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        public T Get<T>(int key) where T : class
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    connection.Open();
                    var data = connection.Get<T>(key);
                    if (data != null)
                        Console.WriteLine("{0} retrieved!", data.GetType());
                    else
                        Console.WriteLine("No object with key: '{0}' was found", key);
                    return data;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        public List<MatchModel> GetPlayerMatches(string playerID)
        {
            List<MatchModel> matches = new List<MatchModel>();

            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    //matches = connection.Query<MatchModel>("SELECT * FROM matches WHERE matches.match_id IN (SELECT played_match.match_id FROM played_match WHERE played_match.player_id = @PlayerID)", new { PlayerID = playerID }).ToList();
                    matches = connection.Query<MatchModel>(
                        "Get_Player_Matches",
                        new { PlayerID = playerID },
                        commandType: CommandType.StoredProcedure
                        ).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            return matches;
        }

        public List<AbilityModel> GetPlayerAbilities(string playerID)
        {
            List<AbilityModel> abilities = new List<AbilityModel>();

            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    //matches = connection.Query<MatchModel>("SELECT * FROM matches WHERE matches.match_id IN (SELECT played_match.match_id FROM played_match WHERE played_match.player_id = @PlayerID)", new { PlayerID = playerID }).ToList();
                    abilities = connection.Query<AbilityModel>(
                        "Get_Player_Abilities",
                        new { PlayerID = playerID },
                        commandType: CommandType.StoredProcedure
                        ).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            return abilities;
        }

        public List<ItemModel> GetPlayerItems(string playerID)
        {
            List<ItemModel> abilities = new List<ItemModel>();

            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    //matches = connection.Query<MatchModel>("SELECT * FROM matches WHERE matches.match_id IN (SELECT played_match.match_id FROM played_match WHERE played_match.player_id = @PlayerID)", new { PlayerID = playerID }).ToList();
                    abilities = connection.Query<ItemModel>(
                        "Get_Player_Items",
                        new { PlayerID = playerID },
                        commandType: CommandType.StoredProcedure
                        ).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            return abilities;
        }

        public List<ItemModel> GetPlayerEquippedItems(string playerID)
        {
            List<ItemModel> abilities = new List<ItemModel>();

            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    abilities = connection.Query<ItemModel>(
                        "Get_Player_Equipped_Items",
                        new { PlayerID = playerID },
                        commandType: CommandType.StoredProcedure
                        ).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            return abilities;
        }


        public List<TestModel> GetAllTest(string playerID)
        {
            List<TestModel> matches = new List<TestModel>();

            using (MySqlConnection connection = CreateConnection())
            {
                matches = connection.Query<TestModel>("SELECT * FROM matches NATURAL JOIN (SELECT * FROM played_match WHERE player_id = @PlayerID) AS matches_played_by_player", new { PlayerID = playerID }).ToList();
            }

            return matches;
        }


        //public AccountModel Insert(AccountModel account)
        //{
        //    using (MySqlConnection connection = CreateConnection())
        //    {
        //        try
        //        {
        //            connection.Open();

        //            MySqlCommand command = connection.CreateCommand();

        //            command.CommandText = "INSERT INTO accounts (account_id, password_hash, email, first_name, last_name) VALUES (@account_id, @password_hash, @email, @first_name, @last_name)";

        //            command.Parameters.AddWithValue("@account_id", account.AccountID);
        //            command.Parameters.AddWithValue("@password_hash", account.PasswordHash);
        //            command.Parameters.AddWithValue("@email", account.Email);
        //            command.Parameters.AddWithValue("@first_name", account.FirstName);
        //            command.Parameters.AddWithValue("@last_name", account.LastName);



        //            try
        //            {
        //                int rowsAffected = command.ExecuteNonQuery();
        //                account.Status = DatabaseResponse.Success;
        //                Console.WriteLine("Account inserted succesfully!");
        //                return account;
        //            }
        //            catch (Exception e)
        //            {
        //                account.Status = DatabaseResponse.AlreadyExists;
        //                Console.WriteLine(e.Message);
        //                return account;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            account.Status = DatabaseResponse.ConnectionFailed;
        //            Console.WriteLine(e.Message);
        //            return account;
        //        }
        //    }
        //}


        //public bool IsConnected()
        //{
        //    if (Connection == null)
        //    {
        //        if (String.IsNullOrEmpty(DatabaseName))
        //            return false;
        //        string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", ServerIP, DatabaseName, Username, Password);
        //        connection = new MySqlConnection(connstring);
        //        connection.Open();
        //    }

        //    return true;
        //}

        //public void Close()
        //{
        //    connection.Close();
        //}
    }
}
