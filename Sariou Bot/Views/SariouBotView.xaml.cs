using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using TwitchLib.Api;
using Sariou_Bot.Models;
using TwitchLib.Api.Core.Models.Undocumented.Chatters;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using System.Diagnostics;
using TwitchLib.Api.Core.Enums;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using Path = System.IO.Path;
using System.Media;
using Sariou_Bot.Components;
using System.Collections.ObjectModel;
using LibVLCSharp.WPF;
using LibVLCSharp.Shared;
using System.Web.Http;
using System.Net.Http;
using Sariou_Bot.ViewModels;

namespace Sariou_Bot.Views
{

    public partial class SariouBotView : UserControl
    {
        public static HttpClient ApiClient { get;  set; }
        //EVENTS
        public static event Action<Boolean> IsBotConnected;
        public static event Action<string> LogEvent;
        ///
        public TwitchAuthViewModel twitchAuth;
       public string TwitchChannelName = "";
       ConnectionCredentials creds = new ConnectionCredentials(BotInfo.BotName, BotInfo.AccessToken);
       public static User broadcaster;
       private TwitchClient? bot;
       public static TwitchAPI? twitchAPI;
       public static Settings? Settings;

       public ObservableCollection<Models.SimpleCommand> simpleCommands = new ObservableCollection<Models.SimpleCommand>();
       public ObservableCollection<Models.SoundCommand> soundCommands = new ObservableCollection<Models.SoundCommand>();
       private Queue<Action> simpleCommandQUE = new Queue<Action>();
       private Queue<Action> soundCommandQUE = new Queue<Action>();

        private List<ChatterFormatted> previousChatters = new List<ChatterFormatted>();
        private List<ChatterFormatted> currentChatters = new List<ChatterFormatted>();
       private ObservableCollection<Viewer> viewersDB = new ObservableCollection<Viewer>();
       private Queue<Action> viewersToAddToDBQUE = new Queue<Action>();

        //For Playing sounds
        LibVLC libVLC = new LibVLC();
        Media media;
        LibVLCSharp.Shared.MediaPlayer mp;
        // //
        public SariouBotView()
        {   
            Settings = DAO.LoadBotSettings()[0];
            InitializeComponent();
            InitializeWebServer();

        }

        private void InitializeWebServer()
        {
            ApiClient = new HttpClient();
        }

        public void SariouBot_Load(object sender, EventArgs e)
       {
            twitchAuth = new TwitchAuthViewModel();
            media = new Media(libVLC, " ");
            mp = new LibVLCSharp.Shared.MediaPlayer(media);
            SetUpTwitchAPI();
           viewersDB = new ObservableCollection<Viewer>(DAO.LoadViewers());
           simpleCommands = new ObservableCollection<Models.SimpleCommand>(DAO.LoadSimpleCommands());
           soundCommands = new ObservableCollection<Models.SoundCommand>(DAO.LoadSoundCommands());

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            RunInBackground(TimeSpan.FromSeconds(1), () => Update());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        private void Update()
        {
            updateCommandsTimer();
            while (simpleCommandQUE.Count > 0)
            {
                Action command = simpleCommandQUE.Dequeue();
                Task.Run(() => command.Invoke());
            }
            if (soundCommandQUE.Count > 0)
            {
                if (!mp.IsPlaying)
                {
                    Action command = soundCommandQUE.Dequeue();
                    command.Invoke();
                }
            }
        }
        private async void PointsUpdate()
        {
            //get chatters
            viewersDB = new ObservableCollection<Viewer>(DAO.LoadViewers());
            previousChatters = currentChatters;
            currentChatters = await twitchAPI.Undocumented.GetChattersAsync(Settings.ChannelName);
            List<ChatterFormatted> viewersToUpdate = new List<ChatterFormatted>();
            foreach (var chatter in currentChatters)
            {
                if (previousChatters.Contains(chatter))
                {
                    viewersToUpdate.Add(chatter);
                }
            }
            //twitchAPI.Helix.Subscriptions.CheckUserSubscriptionAsync();

        }
        private void SetUpTwitchAPI()
        {
            Debug.WriteLine("Setting up TwitchAPI.");
            twitchAPI = new TwitchAPI();
            twitchAPI.Settings.AccessToken = BotInfo.AccessToken;
            twitchAPI.Settings.ClientId = BotInfo.ClientId;
            twitchAPI.Settings.Secret = BotInfo.ClientSecret;
            List<string> broadcasterList = new List<string>() { Settings.ChannelName };
            HomeComponent.ConnectTwitchBot += ConnectTwitchBot;
            HomeComponent.BotDisconnectPressed += Disconnect_Click;
            SimpleCommandsComponent.SimpleCommandAdded += SimpleCommandsComponent_SimpleCommandAdded;
            SoundCommandComponent.SoundCommandAdded += SoundCommandComponent_SoundCommandAdded;
            broadcaster = (twitchAPI.Helix.Users.GetUsersAsync(logins: broadcasterList)).Result.Users[0];
            //var x = twitchAPI.Helix.Subscriptions.CheckUserSubscriptionAsync(broadcaster.Id, "37078197").Result;
        }



        public static Boolean IsFollower(string user)
        {
           return twitchAPI.Helix.Users.GetUsersFollowsAsync(fromId: user , toId: broadcaster.Id).Result.Follows.Length == 1? true:false;
        }
        private void SoundCommandComponent_SoundCommandAdded(SoundCommand command)
        {
            soundCommands.Add(command);
        }
        private void SimpleCommandsComponent_SimpleCommandAdded(SimpleCommand command)
        {
            simpleCommands.Add(command);
        }
        private void ConnectTwitchBot(string channelName)
        {
            {
                TwitchChannelName = channelName;
                bot = new TwitchClient();
                bot.Initialize(creds, TwitchChannelName);
                bot.OnConnected += Client_OnConnected;
                bot.OnDisconnected += Client_Disconnect;
                //bot.OnLog += Client_OnLog;
                bot.OnChatCommandReceived += Client_OnChatCommandReceived;

                bot.OnJoinedChannel += Client_OnJoinedChannel;
                bot.OnUserJoined += Client_OnUserJoined;
                bot.OnUserLeft += Client_OnUserLeft;
                bot.OnMessageReceived += Client_OnMessageReceived;
                bot.OnWhisperReceived += Client_OnWhisperReceived;
                bot.OnNewSubscriber += Client_OnNewSubscriber;
                bot.OnWhisperCommandReceived += Client_OnWhisperCommandReceived;
                bot.Connect();
                
                
            }
        }

        // // Twitch Events // //


        private void Client_OnLog(object? sender, OnLogArgs e)
        {
            Log($"OnLog: {e.Data}");
        }

        private void Client_OnWhisperCommandReceived(object? sender, OnWhisperCommandReceivedArgs e)
        {
        }

        private void Client_OnNewSubscriber(object? sender, OnNewSubscriberArgs e)
        {
        }

        private void Client_OnWhisperReceived(object? sender, OnWhisperReceivedArgs e)
        {
        }

        private void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
        {

        }

        private void Client_OnUserLeft(object? sender, OnUserLeftArgs e)
        {
            Log($"{e.Username}  has Left");
        }

        private void Client_OnUserJoined(object? sender, OnUserJoinedArgs e)
        {
            //TODO: Check if user is in the database. Add them it their not
            Log($"{e.Username}  has joined");
        }

        private void Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
        {
            //bot.SendMessage(TwitchChannelName, Settings.ArrivalMessage);
            GetChannelChatters();

        }

        private void Client_OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
        {
            string commandText = e.Command.CommandText.ToLower();
            //Dynamic command
            foreach (SimpleCommand command in simpleCommands)
            {
                if (commandText.Equals(command.command, StringComparison.OrdinalIgnoreCase))
                {
                    if (GetUserSecurityLevel(e) >= (int)command.permission)
                    {
                        if (!command.onCooldown)
                        {
                            simpleCommandQUE.Enqueue(() => RunSimpleCommand(e));
                            if (command.cooldown > 0)
                            {
                                command.SetOnCooldown(true);
                            }
                        }
                        else
                        {
                            bot.SendMessage(TwitchChannelName, $"sorry {commandText} is on cooldown for another {command.cooldown - command.timer} seconds.");
                        }
                    }
                    break;
                }
            }
            foreach (SoundCommand command in soundCommands)
            {
                if (commandText.Equals(command.Command, StringComparison.OrdinalIgnoreCase))
                {
                    if (GetUserSecurityLevel(e) >= (int)command.Permission)
                    {
                        if (!command.onCooldown)
                        {
                            soundCommandQUE.Enqueue(() => RunSoundCommand(e));
                            if (command.Cooldown > 0)
                            {
                                command.SetOnCooldown(true);
                            }
                        }
                        else
                        {
                            bot.SendMessage(TwitchChannelName, $"sorry {commandText} is on cooldown for another {command.Cooldown - command.timer} seconds.");
                        }
                    }
                    break;
                }
            }
        }
        // // // // // // //
        private int GetUserSecurityLevel(OnChatCommandReceivedArgs e)
        {
            int securityLevel;

            if (e.Command.ChatMessage.IsBroadcaster)
            {
                securityLevel = 4;
                return securityLevel;
            }
            else if (e.Command.ChatMessage.IsModerator)
            {
                securityLevel = 3;
                return securityLevel;
            }
            else if (e.Command.ChatMessage.IsVip)
            {
                securityLevel = 2;
                return securityLevel;
            }
            else if (e.Command.ChatMessage.IsSubscriber)
            {
                securityLevel = 1;
                return securityLevel;
            }
            else
            {
                securityLevel = 0;
                return securityLevel;
            }
        }
        private void Client_Disconnect(object? sender, OnDisconnectedEventArgs e)
        {
            Log($"SariouBot Disconnected!");
            IsBotConnected?.Invoke(false);
        }
        private void Client_OnConnected(object? sender, OnConnectedArgs e)
        {
            Log($"{e.BotUsername} Connected!");
            IsBotConnected?.Invoke(true);


        }
        async public void GetChannelChatters()
        {
          Log($"Getting Chatters Started");
          List<User> users = new List<User>();

          currentChatters = await twitchAPI.Undocumented.GetChattersAsync(Settings.ChannelName);
          List<String> viewerList = new List<String>();
          foreach (var chatter in currentChatters)
          {
              viewerList.Add(chatter.Username);
          }
           Log($"There are {viewerList.Count} viewers in chat.");


          //TODO: CANT DO MORE THAN 100 AT A TIME.. SPLIT IT UP
          var splitCount =(viewerList.Count * 0.01);
          splitCount = Math.Round(splitCount);
          for (int i = 0; i < splitCount; i++)
          {
              int min = 100 * i;
              int max = (100 * i) + 99;
              List<String> slicedList;
              if (viewerList.Count > max)
              {
                  slicedList = viewerList.GetRange(min,100);
              }else{
                  slicedList = viewerList.GetRange(min, viewerList.Count);
              }
              users.AddRange((await twitchAPI.Helix.Users.GetUsersAsync(logins: slicedList)).Users);
              
          }
          foreach (var user in users)
          {
              
              if (viewersDB != null && viewersDB.Count > 0)
              {
                  if (!viewersDB.Any(x => x.Username == user.Login))
                  {
                        //Add new Viewer                   
                        viewersToAddToDBQUE.Enqueue(() => AddNEWUserToViewerDB(user)); 
                  }
              }
              else
              {
                    //Add new Viewer
                    viewersToAddToDBQUE.Enqueue(() => AddNEWUserToViewerDB(user));
                }
            }
            
            while (viewersToAddToDBQUE.Count > 0)
            {
                Action command = viewersToAddToDBQUE.Dequeue();
                Task.Run(() => command.Invoke());
                Log($"Getting Chatters Finished");
            }
           
        }
        private void AddNEWUserToViewerDB(User user)
        {
            DAO.InsertViewerToDB(new Viewer(user.Id, user.Login, user.DisplayName));

        }
        private void updateCommandsTimer()
        {
            foreach (var command in simpleCommands)
            {
                if (command.onCooldown)
                {
                    if (command.timer >= command.cooldown)
                    {
                        command.SetOnCooldown(false);
                        command.ResetTimer();
                    }
                    else
                    {
                        command.UpdateTimer(1);
                    }
                }
            }
            foreach (var command in soundCommands)
            {
                if (command.onCooldown)
                {
                    if (command.timer >= command.Cooldown)
                    {
                        command.SetOnCooldown(false);
                        command.ResetTimer();
                    }
                    else
                    {
                        command.UpdateTimer(1);
                    }
                }
            }
        }
        private void PlaySound(string path)
        {
            media = new Media(libVLC, path);
            mp = new LibVLCSharp.Shared.MediaPlayer(media);
            mp.Play();
        }
        private void Disconnect_Click()
        {
            if (bot.IsConnected)
            {
                //bot.SendMessage(TwitchChannelName, Settings.DepartureMessage);
                Task.Run(() => {
                    bot.Disconnect();
                }).ConfigureAwait(false);

            }
        }
        private void Log(string printMsg)
        {
            LogEvent?.Invoke(printMsg);
            Console.WriteLine(printMsg);
        }
        private void RunSimpleCommand(OnChatCommandReceivedArgs e)
        {
            string commandText = e.Command.CommandText.ToLower();
            //Dynamic command
            foreach (Models.SimpleCommand command in simpleCommands)
            {
                if (commandText.Equals(command.command, StringComparison.OrdinalIgnoreCase))
                {

                    bot.SendMessage(TwitchChannelName, command.content);
                }
            }
        }
        private void RunSoundCommand(OnChatCommandReceivedArgs e)
        {
            string commandText = e.Command.CommandText.ToLower();
            //Dynamic command
            foreach (Models.SoundCommand command in soundCommands)
            {
                if (commandText.Equals(command.Command, StringComparison.OrdinalIgnoreCase))
                {
                    PlaySound(command.FilePath);
                }
            }
        }
        async Task RunInBackground(TimeSpan timeSpan, Action action)
        {
            var periodicTimer = new PeriodicTimer(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                action();
            }
        }
    }
}
