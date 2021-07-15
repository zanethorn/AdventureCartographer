using System.ComponentModel;
using System.Runtime.CompilerServices;
using MapMaker.Annotations;

namespace MapMaker.File
{
    public abstract class MapObject: INotifyPropertyChanged
    {
        private int _pixelWidth;
        private int _pixelHeight;
        private double _offsetX;
        private double _offsetY;
        public event PropertyChangedEventHandler PropertyChanged;

        public int PixelWidth
        {
            get => _pixelWidth;
            set
            {
                if (value == _pixelWidth) return;
                _pixelWidth = value;
                OnPropertyChanged();
            }
        }

        public int PixelHeight
        {
            get => _pixelHeight;
            set
            {
                if (value == _pixelHeight) return;
                _pixelHeight = value;
                OnPropertyChanged();
            }
        }

        public double OffsetX
        {
            get => _offsetX;
            set
            {
                if (value == _offsetX) return;
                _offsetX = value;
                OnPropertyChanged();
            }
        }

        public double OffsetY
        {
            get => _offsetY;
            set
            {
                if (value == _offsetY) return;
                _offsetY = value;
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