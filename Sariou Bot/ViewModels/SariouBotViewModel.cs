using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sariou_Bot.ViewModels
{
    public class SariouBotViewModel : ViewModelBase
    {
        public SimpleCommandsViewModel SimpleCommandsViewModel { get; }
        public SariouBotViewModel()
        {
            SimpleCommandsViewModel = new SimpleCommandsViewModel();
        }

    }
}
