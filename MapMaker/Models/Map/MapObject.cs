using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MapMaker.Models.Map
{
    [DataContract]
    [KnownType(typeof(MapImage))]
    [KnownType(typeof(MapShape))]
    public abstract class MapObject : SmartObject, IRendersBrush
    {
        [DataMember(Name = nameof(X), Order = 1)]
        private int _x;
        
        [DataMember(Name = nameof(Y), Order = 2)]
        private int _y;
        
        [DataMember(Name = nameof(Height), Order = 3)]
        private int _height;
        
        [DataMember(Name = nameof(Width), Order = 4)]
        private int _width;
        

        private Brush? _renderBrush;
        
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
        
        public Size Size
        {
            get => new(Width, Height);
            set
            {
                if (value.Width == Width && value.Height == Height) return;
                Width = (int) value.Width;
                Height = (int) value.Height;
            }
        }
        
        public Point Offset
        {
            get => new(X, Y);
            set
            {
                if (value.X == X && value.Y == Y) return;
                X = (int) value.X;
                Y = (int) value.Y;
            }
        }
        
        public Brush RenderedBrush => _renderBrush ??= GetRenderBrush();

        public abstract Brush GetRenderBrush();

        public override string ToString()
        {
            // Forces children to implement this method
            throw new NotImplementedException();
        }

        protected void OnRenderBrushUpdated()
        {
            _renderBrush = null;
            OnPropertyChanged(nameof(RenderedBrush));
        }
    }
}