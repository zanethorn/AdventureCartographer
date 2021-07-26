using System.Windows;
using System.Windows.Controls;
using MapMaker.Controllers;
using MapMaker.Models.Map;

namespace MapMaker.Views
{
    public partial class BrushButton : Button
    {
        private readonly EditorController _editorController;

        public BrushButton()
        {
            InitializeComponent();
            _editorController = (EditorController) FindResource(nameof(EditorController));
        }

        private void BrushButton_OnClick(object sender, RoutedEventArgs e)
        {
            _editorController.SelectedBrush = (MapBrush) Content;
            _editorController.SelectedToolTray = ToolTrayPanels.Brush;
        }
    }
}