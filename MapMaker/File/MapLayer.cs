using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using MapMaker.Annotations;

namespace MapMaker.File
{
    public class MapLayer: INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<MapObject> _mapObjects = new ObservableCollection<MapObject>();
        private bool _isVisible = true;
        private bool _isLocked =false;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLocked
        {
            get => _isLocked ;
            set
            {
                if (value == _isLocked ) return;
                _isLocked  = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MapObject> MapObjects
        {
            get => _mapObjects;
            set
            {
                if (Equals(value, _mapObjects)) return;
                _mapObjects = value;
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