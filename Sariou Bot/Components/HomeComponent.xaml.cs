using Sariou_Bot.Models;
using Sariou_Bot.ViewModels;
using Sariou_Bot.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace Sariou_Bot.Components
{
    /// <summary>
    /// Interaction logic for HomeComponent.xaml
    /// </summary>
    public partial class HomeComponent : UserControl
    {

        public static event Action<string> ConnectTwitchBot;
        public static event Action BotDisconnectPressed;
        public HomeComponent()
        {
            InitializeComponent();
        }

        public void HomeComponent_Load(object sender, EventArgs e)
        {
            
            ChannelName.Text = SariouBotView.Settings.ChannelName;
            ConnectBot.Visibility = Visibility.Visible;
            DisconnectBot.Visibility = Visibility.Hidden;
            SariouBotView.IsBotConnected += BotConnection;
            SariouBotView.LogEvent += LogEvent;
        }

        private void LogEvent(string printMsg)
        {
            Dispatcher.Invoke(new Action(delegate ()
            {
                ChatBox.AppendText("\r" + printMsg);
            }));
        }

        public void DisconnectFromTwitch(object  sender, EventArgs e)
        {
            DisconnectBot.Content = "Disconnecting...";
            BotDisconnectPressed?.Invoke();
            DisconnectBot.IsEnabled = false;
        }

        public void ConnectToTwitch(object sender, EventArgs e)
        {
            ConnectBot.Content = "Connecting...";
            ConnectTwitchBot?.Invoke(ChannelName.Text);
            SariouBotView.Settings.ChannelName = ChannelName.Text;
            DAO.UpdateSettingsChannelName();
        }

        public void BotConnection(Boolean connection)
        {
            if (connection)
            {
                ShowTwitchDisconnectBtn();
            }else{
                ShowTwitchConnectBtn();
            }
        }

        public void ShowTwitchConnectBtn()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DisconnectBot.Content = "Disconnect";
                DisconnectBot.Visibility = Visibility.Hidden;
                ConnectBot.Content = "Connect";
                ConnectBot.Visibility = Visibility.Visible;

            }));
        }
        public void ShowTwitchDisconnectBtn()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ConnectBot.Content = "Connect";
                ConnectBot.Visibility = Visibility.Hidden;
                DisconnectBot.Content = "Disconnect";
                DisconnectBot.Visibility = Visibility.Visible;
                DisconnectBot.IsEnabled = true;
            }));
        }

        private void TwitchLogin(object sender, RoutedEventArgs e)
        {
            TwitchAuthViewModel twitchAuthViewModel = new TwitchAuthViewModel();
            Twitch_Authentication LoginWindow = new Twitch_Authentication(twitchAuthViewModel);
            LoginWindow.Show();
        }
    }
}
