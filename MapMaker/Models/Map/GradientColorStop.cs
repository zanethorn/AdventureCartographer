using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MapMaker.Models.Map
{
    [DataContract]
    public class GradientColorStop : SmartObject
    {
        [DataMember(Name=nameof(Color), Order=0)]
        private string _color ="#FF000000";
        
        [DataMember(Name=nameof(Offset), Order=1)]
        private double _offset;
        
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

        public Color MediaColor
        {
            get => (Color) ColorConverter.ConvertFromString(Color);
            set => Color = value.ToString();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + Color.GetHashCode();
                hash = hash * 23 + Offset.GetHashCode();
                return hash;
            }
        }
    }
}