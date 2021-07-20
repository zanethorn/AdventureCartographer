using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml.Serialization;
using MapMaker.Annotations;

namespace MapMaker.File
{
    

    [XmlInclude(typeof(MapImage))]
    public abstract class MapObject: INotifyPropertyChanged, ICloneable
    {
        private Size _size;
        private Point _offset;
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlElement()]
        public Size Size
        {
            get => _size;
            set
            {
                if (value == _size) return;
                _size = value;
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
        
        public object Clone()
        {
            var clone = MemberwiseClone();
            OnClone(clone);
            return clone;
        }
        

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected abstract void OnClone(object clone);
    }
}