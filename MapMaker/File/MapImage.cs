using System.Xml.Serialization;
using MapMaker.Library;

namespace MapMaker.File
{
    [XmlType(nameof(MapImage))]
    public class MapImage:MapObject
    {
        [XmlElement]
        public ImageFile Image { get; set; }
    }
}