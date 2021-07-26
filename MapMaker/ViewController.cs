namespace MapMaker
{
    public class ViewController:SmartObject
    {
        private ToolTrayPanels _selectedToolTray;

        public ToolTrayPanels SelectedToolTray
        {
            get => _selectedToolTray;
            set
            {
                if (value == _selectedToolTray) return;
                _selectedToolTray = value;
                OnPropertyChanged();
            }
        }
    }
}