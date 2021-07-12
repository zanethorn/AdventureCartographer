using System.Collections.Generic;
using System.Windows.Media;

namespace MapMaker.File
{
    public class MapLayer
    {
        public string Name { get; set; }
        public Color BackgroundColor { get; set; } = Colors.Transparent;

        public IList<MapObject> MapObjects { get; set; } = new List<MapObject>();
    }
}