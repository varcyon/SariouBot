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
        private int _points;

        public Viewer()
        {
            _viewerID = "";
            _username = "";
            _displayName = "";
            _points = 0;
        }
        public Viewer(string viewerID, string username, string displayName, int points)
        {
            _viewerID = viewerID;
            _username = username;
            _displayName = displayName;
            _points = points;
        }
        public Viewer(string viewerID, string username, string displayName)
        {
            _viewerID = viewerID;
            _username = username;
            _displayName = displayName;

        }
        public string ViewerID { get => _viewerID; set => _viewerID = value; }
        public string Username { get => _username; set => _username = value; }
        public string DisplayName { get => _displayName; set => _displayName = value; }
        public int Points { get => _points; set => _points = value; }
    }
}
