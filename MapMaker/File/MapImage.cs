using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using MapMaker.Library;

namespace MapMaker.File
{
    [XmlType(nameof(MapImage))]
    public class MapImage:MapObject
    {
        public ImageFile Image { get; set; }

        protected override void OnClone(object clone)
        {
            ((MapImage) clone).Image = this.Image;
        }
    }
}