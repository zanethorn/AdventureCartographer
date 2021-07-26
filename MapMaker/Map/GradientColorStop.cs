using System.Windows.Media;
using System.Xml.Serialization;

namespace MapMaker.Map
{
    public class GradientColorStop:SmartObject
    {
        private double _offset;
        private string _color;

        [XmlAttribute]
        public double Offset
        {
            get => _offset;
            set
            {
                if (value.Equals(_offset)) return;
                _offset = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public string Color
        {
            get => _color;
            set
            {
                if (value == _color) return;
                _color = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MediaColor));
            }
        }

        [XmlIgnore]
        public Color MediaColor
        {
            get => (Color)ColorConverter.ConvertFromString(Color);
            set => Color =value.ToString();
        }
    }
}