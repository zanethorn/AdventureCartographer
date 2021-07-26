using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MapMaker.Models.Map
{
    public class MapBrush : SmartObject, IRendersBrush
    {
        private BrushTypes _brushType;
        private SmartCollection<GradientColorStop> _colors = new();
        private double _endX = 1.0;
        private double _endY;
        private IRendersBrush? _nestedBrushRenderer;
        private double _startX;
        private double _startY;

        public MapBrush() : this(System.Windows.Media.Colors.Black)
        {
        }


        public MapBrush(string color) : this(new[] {new GradientColorStop {Color = color}})
        {
        }

        public MapBrush(Color color) : this(new[] {new GradientColorStop {MediaColor = color}})
        {
        }

        public MapBrush(IEnumerable<GradientColorStop> colors)
        {
            Colors = new SmartCollection<GradientColorStop>(colors);
        }

        [XmlAttribute]
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

        [XmlAttribute]
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

        [XmlAttribute]
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

        [XmlAttribute]
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

        [XmlAttribute]
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

        [XmlIgnore]
        [Browsable(false)]
        public Point StartPoint
        {
            get => new(StartX, StartY);
            set
            {
                StartX = value.X;
                StartY = value.Y;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Point EndPoint
        {
            get => new(EndX, EndY);
            set
            {
                EndX = value.X;
                EndY = value.Y;
            }
        }

        [XmlArray]
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

        [XmlElement]
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

        [XmlIgnore]
        public Brush LinearBrush => new LinearGradientBrush(
            new GradientStopCollection(Colors.Select(i => new GradientStop(i.MediaColor, i.Offset))),
            new Point(),
            new Point(1.0, 0.0)
        );

        [XmlIgnore]
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