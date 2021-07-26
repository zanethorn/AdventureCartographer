using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace MapMaker.Views
{
    public partial class PreferencesDialog : Window
    {
        public PreferencesDialog()
        {
            InitializeComponent();
        }

        private void GridSize_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnDefault(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}