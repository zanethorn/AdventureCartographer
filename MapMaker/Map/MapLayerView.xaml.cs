namespace MapMaker.Map
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