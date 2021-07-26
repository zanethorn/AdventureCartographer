using MapMaker.Models.Map;

namespace MapMaker.Views
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