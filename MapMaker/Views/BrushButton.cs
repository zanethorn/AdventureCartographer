using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MapMaker.Controllers;
using MapMaker.Models.Map;
using MapMaker.Views.Panels;

namespace MapMaker.Views
{
    public partial class BrushButton : ButtonBase
    {
        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register(nameof(Brush),typeof(MapBrush), typeof(BrushButton));
        
        private EditorController? _editorController;
        
        protected EditorController EditorController =>
            _editorController ??= (EditorController) FindResource(nameof(EditorController));

        public BrushButton()
        {
            
        }

        public MapBrush Brush
        {
            get => (MapBrush) GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        protected override void OnClick()
        {
            base.OnClick();
            BrushEditor.Instance.TakeControl(this);
            EditorController.SelectedToolTray = ToolTrayPanels.Brush;
        }

    }
}