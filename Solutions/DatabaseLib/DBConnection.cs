using Models;
using System;
using System.Data;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using RandomNameGeneratorLibrary;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using DatabaseREST.Models;

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

        //public T Insert<T>(T data) where T : class
        //{
        //    using (MySqlConnection connection = CreateConnection())
        //    {
        //        try
        //        {
        //            connection.Open();
        //            connection.Insert(data);

        //            //connection.Query<AccountModel>("select * from accounts");
        //            Console.WriteLine("{0} inserted succesfully!", data.GetType());
        //            return data;
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.Message);
        //            return data;
        //        }
        //    }
        //}

        public T Insert<T>(T data) where T : class
        {
            try
            {
                using (var context = new intrusiveContext())
                {

                    context.Add(data);
                    context.SaveChanges();
                }
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            return data;
        }

        public T Update<T>(T data) where T : class
        {
            try
            {
                using (var context = new intrusiveContext())
                {

                    context.Add(data);
                    context.SaveChanges();
                }
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            return data;
        }

        public void EFCORETest()
        {
            try
            {
                using (var context = new intrusiveContext())
                {

                    var temp = context.Accounts.AsNoTracking().FirstOrDefault(acc => acc.AccountId == "TestAcc");
                    var test = context.Accounts.Where(acc => acc.AccountId == "TestAcc");
                    //Console.WriteLine(test.ToQueryString());
                    //var temp = context.Accounts.FirstOrDefault(acc => acc.AccountId == "TestAcc");
                    temp.LastName = "Track test";


                    context.SaveChanges();
                }
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.InnerException.Message);
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

        public List<Matches> GetPlayerMatches(string playerID)
        {
            List<Matches> matches = new List<Matches>();

            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    //matches = connection.Query<Matches>("SELECT * FROM matches WHERE matches.match_id IN (SELECT played_match.match_id FROM played_match WHERE played_match.player_id = @PlayerID)", new { PlayerID = playerID }).ToList();
                    matches = connection.Query<Matches>(
                        "GetPlayerMatches",
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

        public List<Abilities> GetPlayerAbilities(string playerID)
        {
            List<Abilities> abilities = new List<Abilities>();

            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    //matches = connection.Query<Matches>("SELECT * FROM matches WHERE matches.match_id IN (SELECT played_match.match_id FROM played_match WHERE played_match.player_id = @PlayerID)", new { PlayerID = playerID }).ToList();
                    abilities = connection.Query<Abilities>(
                        "GetPlayerAbilities",
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

        public List<Items> GetPlayerItems(string playerID)
        {
            List<Items> abilities = new List<Items>();

            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    //matches = connection.Query<Matches>("SELECT * FROM matches WHERE matches.match_id IN (SELECT played_match.match_id FROM played_match WHERE played_match.player_id = @PlayerID)", new { PlayerID = playerID }).ToList();
                    abilities = connection.Query<Items>(
                        "GetPlayerItems",
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

        public List<Items> GetPlayerEquippedItems(string playerID)
        {
            List<Items> abilities = new List<Items>();

            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    abilities = connection.Query<Items>(
                        "GetPlayerEquippedItems",
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

        public bool UnlockAbility(Players player, Abilities ability)
        {
            //Get the players current abilities
            var playerAbilities = GetPlayerAbilities(player.PlayerId);

            throw new NotImplementedException();

            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    connection.Query(
                    "UnlockAbility",
                    new { PlayerID = player.PlayerId, AbilityName = ability.AbilityName },
                    commandType: CommandType.StoredProcedure
                    ).ToList();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }

        public List<Abilities> GetAllAbilities()
        {
            List<Abilities> abilites = new List<Abilities>();
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    abilites = connection.Query<Abilities>(
                    "GetAllAbilities",
                    new { },
                    commandType: CommandType.StoredProcedure
                    ).ToList();
                    return abilites;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        //public List<TestModel> GetAllTest(string playerID)
        //{
        //    List<TestModel> matches = new List<TestModel>();

        //    using (MySqlConnection connection = CreateConnection())
        //    {
        //        matches = connection.Query<TestModel>("SELECT * FROM matches NATURAL JOIN (SELECT * FROM played_match WHERE player_id = @PlayerID) AS matches_played_by_player", new { PlayerID = playerID }).ToList();
        //    }

        //    return matches;
        //}




        public void InsertRandomData(int amount)
        {
            var personGenerator = new PersonNameGenerator();
            var placeGenerator = new PlaceNameGenerator();

            intrusiveContext context = null;

            Random rnd = new Random(DateTime.Now.Second);

            int index = 10001;

            try
            {
                context = new intrusiveContext();

                for (int i = 0; i < amount; i++)
                {
                    Accounts tempAccount = new Accounts()
                    {
                        AccountId = placeGenerator.GenerateRandomPlaceName() + index.ToString() + personGenerator.GenerateRandomFirstName(),
                        Email = index.ToString(),
                        PasswordHash = index.ToString()
                    };

                    Players tempPlayer = new Players()
                    {
                        Experience = (uint)rnd.Next(10000001),
                        PlayerId = tempAccount.AccountId
                    };


                    context.Add(tempAccount);
                    context.Add(tempPlayer);

                    if (i % 100 == 0)
                    {
                        context.SaveChanges();

                        context.Dispose();
                        context = new intrusiveContext();
                        context.ChangeTracker.AutoDetectChangesEnabled = false;

                    }


                    context.SaveChanges();

                    //connection.Query<AccountModel>("select * from accounts");
                    Console.WriteLine("{0} inserted succesfully!", tempPlayer.PlayerId);
                    index++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }




            //using (MySqlConnection connection = CreateConnection())
            //{
            //    int index = 0;
            //    try
            //    {
            //        connection.Open();

            //        for (int i = 0; i < amount; i++)
            //        {
            //            Accounts tempAccount = new Accounts()
            //            {
            //                AccountId = placeGenerator.GenerateRandomPlaceName() + index.ToString() + personGenerator.GenerateRandomFirstName(),
            //                Email = index.ToString(),
            //                PasswordHash = index.ToString()
            //            };

            //            Players tempPlayer = new Players()
            //            {
            //                Experience = (uint)rnd.Next(10000001),
            //                PlayerId = tempAccount.AccountId
            //            };
            //            connection.Insert(tempAccount);
            //            connection.Insert(tempPlayer);

            //            //connection.Query<AccountModel>("select * from accounts");
            //            Console.WriteLine("{0} inserted succesfully!", tempPlayer.PlayerId);
            //            index++;
            //        }

            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message);
            //    }
            //}
            Console.WriteLine("All inserted");
        }

        private intrusiveContext AddToContext<T>(intrusiveContext context, T entity, int count, int commitCount, bool recreateContext) where T : class
        {
            context.Set<T>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new intrusiveContext();
                    context.ChangeTracker.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }

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

