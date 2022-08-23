using Sariou_Bot.ViewModels;
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
using System.Windows.Shapes;


namespace Sariou_Bot
{
    /// <summary>
    /// Interaction logic for Twitch_Authentication.xaml
    /// </summary>
    public partial class Twitch_Authentication : Window
    {
        public static event Action<AccessTokenReceived> AccessTokenReceived;
        public Twitch_Authentication(TwitchAuthViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            
            TwitchAuthWebBrowser.Source = new Uri(viewModel.AuthRequestURL);
            TwitchAuthWebBrowser.SourceUpdated += TwitchAuthWebBrowser_SourceUpdated;
            URLBOX.Text = TwitchAuthWebBrowser.Source.ToString();
            AccessTokenReceived += RecievedAccessToken;
            AccessTokenReceived?.Invoke(RecievedAccessToken);
        }

        private void RecievedAccessToken(AccessTokenReceived e)
        {
            Close();
        }

        private void TwitchAuthWebBrowser_SourceUpdated(object? sender, DataTransferEventArgs e)
        {
           //Do something;
        }

        public TwitchAuthViewModel ViewModel { get; }

    }
}
