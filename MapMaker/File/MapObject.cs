using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MapMaker.Annotations;

namespace MapMaker.File
{
    public abstract class MapObject: INotifyPropertyChanged
    {
        private double _pixelWidth;
        private double _pixelHeight;
        private Point _offset;
        public event PropertyChangedEventHandler PropertyChanged;

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