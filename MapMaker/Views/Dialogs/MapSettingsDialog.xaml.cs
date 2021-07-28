using System.Windows;

namespace MapMaker.Views.Dialogs
{
    public partial class MapSettingsDialog : Window
    {
        public MapSettingsDialog()
        {
            InitializeComponent();
        }

        private void OnDefault(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}