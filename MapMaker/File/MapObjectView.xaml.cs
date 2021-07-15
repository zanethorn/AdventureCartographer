using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MapMaker.File
{
    public partial class MapObjectView : UserControl
    {
        public MapObjectView()
        {
            InitializeComponent();
            Controller = (MapController)FindResource(nameof(MapController));
        }
        
        public MapController Controller { get; }

        public MapObject MapObject => (MapObject) DataContext;

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Controller.SelectedObject = MapObject;
            content.Stroke = Brushes.Blue;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Controller.SelectedObject = null;
            content.Stroke = Brushes.Transparent;
        }
    }
}