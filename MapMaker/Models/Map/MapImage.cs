using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;
using MapMaker.Models.Library;

namespace MapMaker.Models.Map
{
    [DataContract]
    public class MapImage : MapObject
    {
        [DataMember(Name=nameof(Image), Order = 1001, EmitDefaultValue = true)]
        private LibraryImage? _image;

        public LibraryImage Image
        {
            get => _image!;
            set
            {
                if (Equals(value, _image)) return;
                _image = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RenderedBrush));
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

        public override Brush GetRenderBrush()
        {
            return Image.GetRenderBrush();
        }
    }
}