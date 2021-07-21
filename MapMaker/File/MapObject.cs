using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml.Serialization;
using MapMaker.Annotations;

namespace MapMaker.File
{
    

    [XmlInclude(typeof(MapImage))]
    public abstract class MapObject: SmartObject
    {
        private Size _size;
        private Point _offset;

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
        
        public override string ToString()
        {
            // Forces children to implement this method
            throw new NotImplementedException();
        }
    }
}