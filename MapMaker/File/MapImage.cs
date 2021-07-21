using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using MapMaker.Library;

namespace MapMaker.File
{
    [XmlType(nameof(MapImage))]
    public class MapImage:MapObject
    {
        private ImageFile _image;

        public ImageFile Image
        {
            get => _image;
            set
            {
                if (Equals(value, _image)) return;
                _image = value;
                OnPropertyChanged();
            }
        }

        protected override void OnClone(object clone)
        {
            var myClone = (MapImage) clone;
            myClone._image = _image;
        }

        public override string ToString()
        {
            return $"Image ({Image.ShortName})";
        }
    }
}