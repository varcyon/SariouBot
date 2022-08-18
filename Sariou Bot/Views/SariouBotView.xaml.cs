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
using TwitchLib.Api.Core.Models.Undocumented.ChatUser;
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

namespace Sariou_Bot.Views
{

    public partial class SariouBotView : UserControl
    {
        //EVENTS
        public static event Action<Boolean> IsBotConnected;
        ///

       public string TwitchChannelName = "";
       ConnectionCredentials creds = new ConnectionCredentials(BotInfo.BotName, BotInfo.AccessToken);
       private TwitchClient? bot;
       public TwitchAPI? twitchAPI;



       public List<Models.SimpleCommand> simpleCommands = new List<Models.SimpleCommand>();
       public List<Models.SoundCommand> soundCommands = new List<Models.SoundCommand>();
       public Boolean simpleCommandsGridNeedsRefresh = false;
       public Boolean soundCommandsGridNeedsRefresh = false;
       private List<Viewer> viewersDB = new List<Viewer>();
       private List<Viewer> viewersToAddToDB = new List<Viewer>();
       private Queue<Action> commandsQUE = new Queue<Action>();

        public SariouBotView()
        {
            InitializeComponent();
        }

        public void SariouBot_Load(object sender, EventArgs e)
       {
           SetUpTwitchAPI();
           //TODO: Get VIEWERDB
           simpleCommands = new List<Models.SimpleCommand>(DAO.LoadSimpleCommands());


            RunInBackground(TimeSpan.FromSeconds(1), () => Update());
       }
        private void SetUpTwitchAPI()
        {
            Debug.WriteLine("Setting up TwitchAPI.");
            twitchAPI = new TwitchAPI();
            twitchAPI.Settings.AccessToken = BotInfo.AccessToken;
            twitchAPI.Settings.ClientId = BotInfo.ClientId;
            twitchAPI.Settings.Secret = BotInfo.ClientSecret;

            HomeComponent.ConnectTwitchBot += ConnectTwitchBot;
            HomeComponent.BotDisconnectPressed += Disconnect_Click;
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
                bot.OnExistingUsersDetected += Client_OnExistingUsersDetected;

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

    




private void Client_OnExistingUsersDetected(object? sender, OnExistingUsersDetectedArgs e)
{
  var exisitingUsers = e.Users;
  foreach (var user in exisitingUsers)
  {
      Log($"{user}");
  }
}

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
  Log($"{e.ChatMessage.DisplayName}: {e.ChatMessage.UserId} {e.ChatMessage.IsModerator} {e.ChatMessage.IsVip} {e.ChatMessage.IsBroadcaster} {e.ChatMessage.IsSubscriber}");
}

private void Client_OnUserLeft(object? sender, OnUserLeftArgs e)
{
  Log($"{e.Username}  has Left");

}

private void Client_OnUserJoined(object? sender, OnUserJoinedArgs e)
{
  Log($"{e.Username}  has joined");
}

private void Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
{
  bot.SendMessage(TwitchChannelName, "I have arrived!");

}

private void Client_OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
{
  commandsQUE.Enqueue(() => RunCommand(e));

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
        /*
        private void AddSimpleCommandBtn(object sender, RoutedEventArgs e)
        {
          Models.SimpleCommand commandToAdd = new Models.SimpleCommand(CommandName.Text,(isAutomated.IsChecked == true)? 1:0,float.Parse(CommandCooldown.Text), CommandContent.Text, CommandPermissions.SelectedIndex, DateTime.Now.ToString());
          DAO.SaveSimpleChatCommand(commandToAdd);
          simpleCommandsGridNeedsRefresh = true;
        }
        private void AddSoundCommandBtn(object sender, RoutedEventArgs e)
        {
          Models.SoundCommand commandToAdd = new Models.SoundCommand(CommandName.Text, SoundCommandFilePath.Text, CommandPermissions.SelectedIndex,float.Parse(SoundCommandCooldown.Text), DateTime.Now.ToString());
          DAO.SaveSoundCommand(commandToAdd);
          soundCommandsGridNeedsRefresh = true;
        }


        async public void GetChannelChatters(object sender, RoutedEventArgs e)
        {
          Log($"Getting Chatters Started");
          TwitchChannelName = ChannelName.Text;
          List<User> users = new List<User>();

          var currentChatters = await twitchAPI.Undocumented.GetChattersAsync(TwitchChannelName);
          List<String> viewerList = new List<String>();
          foreach (var chatter in currentChatters)
          {
              viewerList.Add(chatter.Username);
          }


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
                  slicedList = viewerList.GetRange(min,max );
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
                      AddNEWUserToViewerDB(user);
                  }
              }
              else
              {
                  //Add new Viewer
                  AddNEWUserToViewerDB(user);
              }
          }

          Log($"Getting Chatters Finished");
        }


        private void autoCheckChanged(object sender, RoutedEventArgs e)
        {
          if ((bool)isAutomated.IsChecked)
          {
              CommandCooldown.IsEnabled = true;
          }
          else { CommandCooldown.IsEnabled = false; }
        }


        private void AddNEWUserToViewerDB(User user)
        {
          viewersToAddToDB.Add(new Viewer(user.Id, user.Login, user.DisplayName, 0));
        }

 



        // Refresh the data grid to show timer.. DO WE WANT / NEED this?
        private void refreshSimpleCommandsGrid()
        {
          SimpleCommands.ItemsSource = null;
          simpleCommands = new List<Models.SimpleCommand>(DAO.LoadSimpleCommands());
          SimpleCommands.ItemsSource = simpleCommands;
        }
        private void refreshSoundCommandsGrid()
        {
          SoundCommands.ItemsSource = null;
          soundCommands = new List<Models.SoundCommand>(DAO.LoadSoundCommands());
          SoundCommands.ItemsSource = soundCommands;
        }
        private void updateCommandsTimer()
        {
          foreach (var command in simpleCommands)
          {
              if (command.onCooldown)
              {
                  if(command.timer >= command.cooldown)
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
        private void OpenSoundFileDialog(object sender, RoutedEventArgs e)
        {
          OpenFileDialog openFileDialog = new OpenFileDialog();
          openFileDialog.InitialDirectory = Environment.CurrentDirectory;
          openFileDialog.Filter = "Sound Files (*.wav)|*.wav";
          if (openFileDialog.ShowDialog() == true)
              SoundCommandFilePath.Text = Path.GetFullPath(openFileDialog.FileName);
        }

        private void PlaySound(string path)
        {
          SoundPlayer soundCommand = new SoundPlayer(path);
          soundCommand.PlaySync();
        }

        */
        private void Disconnect_Click()
        {
            if (bot.IsConnected)
            {
                bot.SendMessage(TwitchChannelName, "Taking the blue pill!");
                Task.Run(() => {
                    bot.Disconnect();
                }).ConfigureAwait(false);

            }
        }
        private void Log(string printMsg)
        {

            Dispatcher.Invoke(new Action(delegate ()
            {
               // ChatBox.AppendText("\r" + printMsg);
            }));
            Console.WriteLine(printMsg);
        }
        private void RunCommand(OnChatCommandReceivedArgs e)
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
        private void Update()
        {
            if (simpleCommandsGridNeedsRefresh)
            {
                simpleCommandsGridNeedsRefresh = false;
                //refreshSimpleCommandsGrid();
            }
            if (soundCommandsGridNeedsRefresh)
            {
                soundCommandsGridNeedsRefresh = false;
                //refreshSoundCommandsGrid();
            }
            //deque command and run it on a Task
            if (commandsQUE.Count > 0)
            {
                Action command = commandsQUE.Dequeue();
                Task.Run(() => command.Invoke());
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
