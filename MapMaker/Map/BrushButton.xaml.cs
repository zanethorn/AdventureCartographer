using System.Windows;
using System.Windows.Controls;

namespace MapMaker.Map
{
    public partial class BrushButton :Button
    {
        private readonly MapController _mapController;
        private readonly ViewController _viewController;
        
        public BrushButton()
        {
            InitializeComponent();
            _mapController = (MapController) FindResource(nameof(MapController));
            _viewController = (ViewController) FindResource(nameof(ViewController));
        }

        private void BrushButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mapController.SelectedBrush = (MapBrush)Content;
            _viewController.SelectedToolTray = ToolTrayPanels.Brush;
        }
    }
}