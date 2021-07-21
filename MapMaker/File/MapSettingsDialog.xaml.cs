using System.Windows;

namespace MapMaker.File
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