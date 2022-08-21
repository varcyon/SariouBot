using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sariou_Bot.Models
{
    public  class Settings
    {
        private string? channelName;
        private string? arrivalMessage;
        private string? departureMessage;
        private string? pointsName;
        private int? pointsPerMinute;
        private int? subPointMultiplier;
        private int? subGivewayMultiplier;
        public string? ChannelName { get => channelName; set =>channelName = value; }
        public string? ArrivalMessage { get => arrivalMessage; set => arrivalMessage = value; }
        public string? DepartureMessage { get => departureMessage; set => departureMessage = value; }
    }
}
