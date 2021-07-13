using System.ComponentModel;
using System.Runtime.CompilerServices;
using MapMaker.Annotations;

namespace MapMaker.File
{
    public class MapController: INotifyPropertyChanged
    {
        private MapFile _mapFile = new MapFile();
        private float _scale = 0.5f;
        private int _offsetX;
        private int _offsetY;
        public event PropertyChangedEventHandler PropertyChanged;

        public float Scale
        {
            get => _scale;
            set
            {
                if (value.Equals(_scale)) return;
                _scale = value;
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


        public MapFile MapFile
        {
            get => _mapFile;
            set
            {
                if (Equals(value, _mapFile)) return;
                _mapFile = value;
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