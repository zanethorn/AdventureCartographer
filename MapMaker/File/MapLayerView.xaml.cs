using System.Windows.Controls;
using System.Windows.Input;

namespace MapMaker.File
{
    public partial class MapLayerView : UserControl
    {
        public MapLayerView()
        {
            InitializeComponent();
        }

        public MapLayer Layer => (MapLayer) DataContext;
        
        
    }
}