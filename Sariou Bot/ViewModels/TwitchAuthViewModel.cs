using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sariou_Bot.ViewModels
{
    public class TwitchAuthViewModel : ViewModelBase
    {
        private string ClientID { get; } = BotInfo.ClientId;
        private string RedirectURL { get; } = "";
        private string[] Scopes ;
        private string ScopeString ;
        public static readonly string State = Guid.NewGuid().ToString();
        public string AuthRequestURL;

        public TwitchAuthViewModel()
        {
            Scopes = File.ReadAllLines("Scopes.txt").ToArray();
            ScopeString = string.Join("+", value: Scopes);
            AuthRequestURL = $"https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={ClientID}&redirect_uri={RedirectURL}&scope={ScopeString}&state={State}";
        }


    }
}
