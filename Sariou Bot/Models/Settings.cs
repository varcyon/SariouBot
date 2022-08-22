using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sariou_Bot.Models
{
    public class Settings
    {
        private string? channelName;
        private string? arrivalMessage;
        private string? departureMessage;
        private string? pointsName;
        private long? pointsPerMinute;
        private long? subPointMultiplier;
        private long? subGivewayMultiplier;



        public string? ChannelName { get => channelName; set =>channelName = value; }
        public string? ArrivalMessage { get => arrivalMessage; set => arrivalMessage = value; }
        public string? DepartureMessage { get => departureMessage; set => departureMessage = value; }
        public string? PointsName { get => pointsName; set => pointsName = value; }
        public long? PointsPerMinute { get => pointsPerMinute; set => pointsPerMinute = value; }
        public long? SubPointMultiplier { get => subPointMultiplier; set => subPointMultiplier = value; }
        public long? SubGivewayMultiplier { get => subGivewayMultiplier; set => subGivewayMultiplier = value; }
    }
}
