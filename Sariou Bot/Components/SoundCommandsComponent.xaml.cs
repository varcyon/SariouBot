using Microsoft.Win32;
using Sariou_Bot.Models;
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
using System.IO;
namespace Sariou_Bot.Components
{
    /// <summary>
    /// Interaction logic for SoundCommandComponent.xaml
    /// </summary>
    public partial class SoundCommandComponent : UserControl
    {
        public static event Action<SoundCommand> SoundCommandAdded;
        public SoundCommandComponent()
        {
            InitializeComponent();
        }
        private void OpenSoundFileDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            //openFileDialog.Filter = "Sound Files (*.wav)|*.wav";
            if (openFileDialog.ShowDialog() == true)
                SoundCommandFilePath.Text = Path.GetFullPath(openFileDialog.FileName);
        }
        private void AddSoundCommandBtn(object sender, RoutedEventArgs e)
        {
            SoundCommand command = new SoundCommand(SoundCommandName.Text,
                SoundCommandFilePath.Text,
                SoundCommandPermissions.SelectedIndex,
                float.Parse(SoundCommandCooldown.Text));
            DAO.SaveSoundCommand(command);

            SoundCommands.ItemsSource = new ObservableCollection<Models.SoundCommand>(DAO.LoadSoundCommands());
            SoundCommands.Items.Refresh();
            SoundCommandAdded?.Invoke(command);
        }
    }
}
