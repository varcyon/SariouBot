using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sariou_Bot.ViewModels
{
    public class SoundCommandsViewModel : ViewModelBase
    {
        private ObservableCollection<Models.SoundCommand> _soundComandsViewModel;
        public IEnumerable<Models.SoundCommand> soundCommandsViewModel => _soundComandsViewModel;

        public SoundCommandsViewModel()
        {
            _soundComandsViewModel = new ObservableCollection<Models.SoundCommand>(DAO.LoadSoundCommands());
        }
    }
}
