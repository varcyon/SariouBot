using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;
using Sariou_Bot.Models;
using Sariou_Bot.Views;

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

        public static List<Settings> LoadBotSettings()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var result = conn.Query<Settings>("SELECT * FROM BotSettings LIMIT 1", new DynamicParameters());
                return (List<Settings>)result;
            }
        }
        public static void UpdateSettingsChannelName()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string updateString = @"UPDATE BotSettings SET ChannelName = @Channel";
                conn.Execute(updateString, new
                {
                    @Channel = SariouBotView.Settings.ChannelName
                }); ;
            }

        }
        public static List<Viewer> LoadViewers()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var result = conn.Query<Viewer>("SELECT * FROM Viewers", new DynamicParameters());
                return (List<Viewer>)result;
            }
        }
        public static void InsertViewerToDB(Viewer viewer)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string insertQuery = @"INSERT into Viewers (ViewerID,Username,DisplayName) VALUES (@ViewerID,@Username,@DisplayName)";
                var result = conn.Execute(insertQuery, new
                {
                    @ViewerID = viewer.ViewerID,
                    @Username = viewer.Username,
                    @DisplayName = viewer.DisplayName
                });

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
                    @Filepath = command.FilePath,
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
