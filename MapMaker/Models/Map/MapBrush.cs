using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using MapMaker.Models.Library;

namespace MapMaker.Models.Map
{
    [DataContract]
    [KnownType(typeof(LibraryImage))]
    [KnownType(typeof(MapObject))]
    public class MapBrush : SmartObject, IRendersBrush
    {
        [DataMember(Name = nameof(BrushType), Order=0)]
        private BrushTypes _brushType;
        
        [DataMember(Name=nameof(StartX), Order=1)]
        private double _startX;
        
        [DataMember(Name=nameof(StartY), Order =2)]
        private double _startY;
        
        [DataMember(Name=nameof(EndX), Order=3)]
        private double _endX = 1.0;
        
        [DataMember(Name=nameof(EndY), Order=4)]
        private double _endY;
        
        [DataMember(Name=nameof(EndY), Order=4)]
        private string _name = string.Empty;
        
        [DataMember(Name=nameof(Colors), Order=1001)]
        private SmartCollection<GradientColorStop> _colors = new();
        
        [DataMember(Name=nameof(NestedBrushRenderer), Order = 2001, EmitDefaultValue = false)]
        private IRendersBrush? _nestedBrushRenderer;

        


        public MapBrush() : this(System.Windows.Media.Colors.Black)
        {
        }


        public MapBrush(string color) : this((Color)ColorConverter.ConvertFromString(color))
        {
        }

        public MapBrush(Color color) : this(new[] {new GradientColorStop {MediaColor = color}})
        {
            var colorProperty = typeof(Colors).GetProperties()
                .FirstOrDefault(p => Color.AreClose((Color)p.GetValue(null), color));
            Name =  colorProperty != null ? colorProperty.Name : color.ToString();
        }

        public MapBrush(IEnumerable<GradientColorStop> colors)
        {
            Colors = new SmartCollection<GradientColorStop>(colors);
        }
        
        public BrushTypes BrushType
        {
            get => _brushType;
            set
            {
                if (value == _brushType) return;
                _brushType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
            }
        }
        
        public double StartX
        {
            get => _startX;
            set
            {
                if (value.Equals(_startX)) return;
                _startX = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StartPoint));
                OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
            }
        }
        
        public double StartY
        {
            get => _startY;
            set
            {
                if (value.Equals(_startY)) return;
                _startY = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StartPoint));
                OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
            }
        }
        
        public double EndX
        {
            get => _endX;
            set
            {
                if (value.Equals(_endX)) return;
                _endX = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EndPoint));
                OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
            }
        }
        
        public double EndY
        {
            get => _endY;
            set
            {
                if (value.Equals(_endY)) return;
                _endY = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EndPoint));
                OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
            }
        }
        
        public Point StartPoint
        {
            get => new(StartX, StartY);
            set
            {
                StartX = value.X;
                StartY = value.Y;
            }
        }
        
        public Point EndPoint
        {
            get => new(EndX, EndY);
            set
            {
                EndX = value.X;
                EndY = value.Y;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + BrushType.GetHashCode();
                hash = hash * 23 + StartX.GetHashCode();
                hash = hash * 23 + StartY.GetHashCode();
                hash = hash * 23 + EndX.GetHashCode();
                hash = hash * 23 + EndY.GetHashCode();

                foreach (var color in Colors)
                {
                    hash = hash * 23 + color.GetHashCode();
                }

                return hash;
            }
        }

        public SmartCollection<GradientColorStop> Colors
        {
            get => _colors;
            set
            {
                if (Equals(value, _colors)) return;
                _colors.CollectionChanged -= OnColorsCollectionChanged;
                foreach (var i in _colors)
                    i.PropertyChanged -= OnColorsChanged;
                _colors = value;
                _colors.CollectionChanged += OnColorsCollectionChanged;
                foreach (var i in _colors)
                    i.PropertyChanged += OnColorsChanged;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
                OnPropertyChanged(nameof(LinearBrush));
            }
        }
        
        public IRendersBrush? NestedBrushRenderer
        {
            get => _nestedBrushRenderer;
            set
            {
                if (Equals(value, _nestedBrushRenderer)) return;
                _nestedBrushRenderer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
                OnPropertyChanged(nameof(LinearBrush));
            }
        }
        
        public Brush LinearBrush => new LinearGradientBrush(
            new GradientStopCollection(Colors.Select(i => new GradientStop(i.MediaColor, i.Offset))),
            new Point(),
            new Point(1.0, 0.0)
        );
        
        public Brush RenderedBrush => GetRenderBrush();

        public Brush GetRenderBrush()
        {
            return BrushType switch
            {
                BrushTypes.Solid => new SolidColorBrush(Colors[0].MediaColor),
                BrushTypes.LinearGradient => new LinearGradientBrush(
                    new GradientStopCollection(Colors.Select(i => new GradientStop(i.MediaColor, i.Offset))),
                    StartPoint,
                    EndPoint
                ),
                BrushTypes.RadialGradient => new RadialGradientBrush(
                    new GradientStopCollection(Colors.Select(i => new GradientStop(i.MediaColor, i.Offset)))),
                BrushTypes.Image => NestedBrushRenderer!.GetRenderBrush(),
                BrushTypes.Object => NestedBrushRenderer!.GetRenderBrush(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void OnColorsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
            if (e.NewItems != null)
                foreach (GradientColorStop i in e.NewItems)
                    i.PropertyChanged += OnColorsChanged;
            if (e.OldItems != null)
                foreach (GradientColorStop i in e.OldItems)
                    i.PropertyChanged -= OnColorsChanged;
        }

        private void OnColorsChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
            OnPropertyChanged(nameof(LinearBrush));
        }
    }
}