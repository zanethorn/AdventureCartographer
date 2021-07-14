using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MapMaker.Properties;

namespace MapMaker
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
            Settings.Default.Save();
            DialogResult = true;
        }

        private void PenColor_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.Default.GridPenColor = dialog.Color.ToString();
            }
        }
    }
}