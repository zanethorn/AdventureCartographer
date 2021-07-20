using System.Windows.Controls;
using System.Windows.Input;

namespace MapMaker.File
{
    public partial class MapLayerView 
    {
        public MapLayerView()
        {
            InitializeComponent();
        }

        public MapLayer Layer => (MapLayer) DataContext;
        
        
    }
}