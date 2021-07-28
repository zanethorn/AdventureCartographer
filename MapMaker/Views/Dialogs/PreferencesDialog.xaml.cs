using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MapMaker.Controllers;
using MapMaker.Models;
using MonitoredUndo;

namespace MapMaker.Views.Dialogs
{
    public partial class PreferencesDialog : Window
    {
        private readonly UndoBatch _undo;
        private readonly SettingsController _settingsController;
        
        public PreferencesDialog()
        {
            InitializeComponent();

            _settingsController = (SettingsController) FindResource(nameof(SettingsController));
            _undo = new UndoBatch(_settingsController.Settings, "Settings Rollback", true);
        }

        private void GridSize_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnDefault(object sender, RoutedEventArgs e)
        {
            _undo.Dispose();
            _settingsController.Save();
            UndoService.Current[_settingsController].Clear();
            DialogResult = true;
        }


        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _undo.Dispose();
            UndoService.Current[_settingsController].Undo();
            DialogResult = false;
        }
    }
}