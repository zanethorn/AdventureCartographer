using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapMaker.Models.Map;

namespace MapMaker.Views.Library
{
    public partial class MapBrushView : UserControl
    {
        public MapBrushView()
        {
            InitializeComponent();
        }
        
        public MapBrush Brush => (MapBrush) DataContext;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragDrop.DoDragDrop(this, Brush, DragDropEffects.Link);
        }
    }
}