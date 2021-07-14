using System.ComponentModel;
using System.Runtime.CompilerServices;
using MapMaker.Annotations;

namespace MapMaker.File
{
    public abstract class MapObject: INotifyPropertyChanged
    {
        private int _pixelWidth;
        private int _pixelHeight;
        private int _offsetX;
        private int _offsetY;
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

        public int OffsetX
        {
            get => _offsetX;
            set
            {
                if (value == _offsetX) return;
                _offsetX = value;
                OnPropertyChanged();
            }
        }

        public int OffsetY
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