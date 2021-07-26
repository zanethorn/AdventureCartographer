using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MapMaker.Map
{
    

    [XmlInclude(typeof(MapImage))]
    public abstract class MapObject: SmartObject, IRendersBrush
    {
        private Brush? _renderBrush;
        private int _height;
        private int _width;
        private int _y;
        private int _x;

        [XmlAttribute]
        public int X
        {
            get => _x;
            set
            {
                if (value == _x) return;
                _x = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Offset));
            }
        }

        [XmlAttribute]
        public int Y
        {
            get => _y;
            set
            {
                if (value == _y) return;
                _y = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Offset));
            }
        }

        [XmlAttribute]
        public int Width
        {
            get => _width;
            set
            {
                if (value == _width) return;
                _width = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Size));
            }
        }

        [XmlAttribute]
        public int Height
        {
            get => _height;
            set
            {
                if (value == _height) return;
                _height = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Size));
            }
        }

        [XmlIgnore()]
        [Browsable(false)]
        public Size Size
        {
            get => new (Width , Height);
            set
            {
                if (value.Width == Width && value.Height == Height) return;
                Width = (int)value.Width;
                Height = (int)value.Height;
            }
        }

        [XmlIgnore()]
        [Browsable(false)]
        public Point Offset
        {
            get => new (X,Y);
            set
            {
                if (value.X == X && value.Y == Y) return;
                X = (int)value.X;
                Y = (int) value.Y;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Brush RenderedBrush => _renderBrush ??= GetRenderBrush();

        public override string ToString()
        {
            // Forces children to implement this method
            throw new NotImplementedException();
        }

        public abstract Brush GetRenderBrush();

        protected void OnRenderBrushUpdated()
        {
            _renderBrush = null;
            OnPropertyChanged(nameof(RenderedBrush));
        }
    }
}