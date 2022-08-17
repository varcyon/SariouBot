using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;
using Sariou_Bot.Models;

namespace Sariou_Bot
{
    public class DAO
    {
        public static List<SimpleCommand> LoadSimpleCommands()
        {
            using(IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var sqlResult = conn.Query<SimpleCommand>("SELECT * FROM SimpleChatCommands", new DynamicParameters());
                return sqlResult.ToList();
            } 
        }
        public static List<SoundCommand> LoadSoundCommands()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var sqlResult = conn.Query<SoundCommand>("SELECT * FROM SoundCommands", new DynamicParameters());
                return sqlResult.ToList();
            }
        }
        public static void AddNewUsers(List<Viewer> listToAdd)
        {
            List<Task> tasks = new List<Task>();
            foreach (Viewer viewer in listToAdd)
            {
                tasks.Add(Task.Run(() =>
                {

                }
                ));
            }

            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Query<SimpleCommand>("SELECT * FROM SimpleChatCommands", new DynamicParameters());    
                
            }
        }
        public static void InsertViewerToDB(Viewer viewer)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("INSERT into SimpleChatCommands (Command,Content) VALUES (@Command,@Content)", viewer);
            }
        }

        public static void SaveSimpleChatCommand(SimpleCommand command)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string insertQuery = @"INSERT INTO SimpleChatCommands (Command,Content,Permission,Automated,Cooldown,CreatedOn) VALUES (@Command, @Content, @Permission, @Automated, @Cooldown, @CreatedOn)";
                var result = conn.Execute(insertQuery, new
                {
                   @Command= command.command,
                   @Content= command.content,
                   @Permission = (int)command.permission,
                   @Automated = (int)command.auto,
                   @Cooldown = command.cooldown,
                   @CreatedOn = DateTime.Now.ToString()
                });
               
            }
        }
        public static void SaveSoundCommand(SoundCommand command)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string insertQuery = @"INSERT INTO SoundCommands (Command,Filepath,Permission,Cooldown,CreatedOn) VALUES (@Command, @Filepath, @Permission, @Cooldown, @CreatedOn)";
                var result = conn.Execute(insertQuery, new
                {
                    @Command = command.Command,
                    @Filepath = command.SoundFile,
                    @Permission = (int)command.Permission,
                    @Cooldown = command.Cooldown,
                    @CreatedOn = DateTime.Now.ToString()
                });

            }
        }



        private static string LoadConnectionString(string id="Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
