using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml.Serialization;
using MapMaker.Annotations;

namespace MapMaker.File
{
    

    [XmlInclude(typeof(MapImage))]
    public abstract class MapObject: INotifyPropertyChanged
    {
        private double _pixelWidth;
        private double _pixelHeight;
        private Point _offset;
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute]
        public double PixelWidth
        {
            get => _pixelWidth;
            set
            {
                if (value == _pixelWidth) return;
                _pixelWidth = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public double PixelHeight
        {
            get => _pixelHeight;
            set
            {
                if (value == _pixelHeight) return;
                _pixelHeight = value;
                OnPropertyChanged();
            }
        }

        [XmlElement]
        public Point Offset
        {
            get => _offset;
            set
            {
                if (value == _offset) return;
                _offset = value;
                OnPropertyChanged();
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}