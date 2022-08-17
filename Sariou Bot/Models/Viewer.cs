using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sariou_Bot.Models
{
    public class Viewer
    {
        private string _viewerID;
        private string _username;
        private string _displayName;

        private Boolean _isMod;
        private Boolean _isSub;
        private Boolean _isVIP;
        private Boolean _isBroadcaster;

        private int _points;

        public Viewer(string viewerID, string username, string displayName, int points)
        {
            _viewerID = viewerID;
            _username = username;
            _displayName = displayName;
            _points = points;
        }

        public string ViewerID { get => _viewerID; set => _viewerID = value; }
        public string Username { get => _username; set => _username = value; }
        public string DisplayName { get => _displayName; set => _displayName = value; }
        public bool IsMod { get => _isMod; set => _isMod = value; }
        public bool IsSub { get => _isSub; set => _isSub = value; }
        public bool IsVIP { get => _isVIP; set => _isVIP = value; }
        public bool IsBroadcaster { get => _isBroadcaster; set => _isBroadcaster = value; }
        public int Points { get => _points; set => _points = value; }
    }
}
