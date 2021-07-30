using System.Windows;
using System.Windows.Controls;
using MapMaker.Controllers;
using MapMaker.Models.Map;
using MapMaker.Views.Panels;

namespace MapMaker.Views
{
    public partial class BrushButton : Button
    {
        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register(nameof(Brush),typeof(MapBrush), typeof(BrushButton));
        
        private readonly EditorController _editorController;

        public BrushButton()
        {
            InitializeComponent();
            _editorController = (EditorController) FindResource(nameof(EditorController));
        }

        public MapBrush Brush
        {
            get => (MapBrush) GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }
        
        private void BrushButton_OnClick(object sender, RoutedEventArgs e)
        {
            BrushEditor.Instance.TakeControl(this);
            _editorController.SelectedToolTray = ToolTrayPanels.Brush;
        }
    }
}