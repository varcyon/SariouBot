using Sariou_Bot.Models;
using Sariou_Bot.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sariou_Bot.Components
{
    /// <summary>
    /// Interaction logic for SimpleCommandsComponent.xaml
    /// </summary>
    public partial class SimpleCommandsComponent : UserControl
    {
        public static event Action<SimpleCommand> SimpleCommandAdded;
        public SimpleCommandsComponent()
        {
            InitializeComponent();
        }

        private void AddSimpleCommand(object sender, RoutedEventArgs e)
        {
            SimpleCommand command = new SimpleCommand(CommandName.Text,(isAutomated.IsChecked?? false) ? 1:0,float.Parse(CommandCooldown.Text),CommandContent.Text,CommandPermissions.SelectedIndex);
            DAO.SaveSimpleChatCommand(command);
            SimpleCommands.ItemsSource = new ObservableCollection<Models.SimpleCommand>(DAO.LoadSimpleCommands());

            SimpleCommands.Items.Refresh();
            SimpleCommandAdded?.Invoke(command);
        }
    }
}
