using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sariou_Bot.ViewModels
{
    public class SimpleCommandsViewModel : ViewModelBase
    {
        private  ObservableCollection<Models.SimpleCommand> _simpleComandsViewModel;
        public IEnumerable<Models.SimpleCommand> simpleCommandsViewModel => _simpleComandsViewModel;

        public SimpleCommandsViewModel()
        {
            _simpleComandsViewModel = new ObservableCollection<Models.SimpleCommand>(DAO.LoadSimpleCommands());
        }

    }
}
