using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using Point = System.Windows.Point;


namespace MapMaker.Models.Map
{
    [DataContract]
    public class MapText : MapObject
    {
        private static readonly MapMaker.Converters.FontStyleConverter FontStyleConverter = new ();
        private static readonly MapMaker.Converters.FontWeightConverter FontWeightConverter = new ();
        private static readonly MapMaker.Converters.FontStretchConverter FontStretchConverter = new ();
        
        [DataMember(Name = nameof(Text), Order = 1001)]
        private string _text="string.Empty";
        
        [DataMember(Name = nameof(Text), Order = 1002)]
        private string _fontFamily = "Ariel";
        
        [DataMember(Name = nameof(Text), Order = 1003)]
        private double _fontSize =16.0;

        [DataMember(Name = nameof(FlowDirection), Order=1004)]
        private FlowDirection _flowDirection = FlowDirection.LeftToRight;
        
        [DataMember(Name = nameof(FontStyle), Order=1005)]
        private FontStyle _fontStyle= FontStyle.Normal;
        
        [DataMember(Name = nameof(FlowDirection), Order=1006)]
        private FontWeight _fontWeight = FontWeight.Normal;
        
        [DataMember(Name = nameof(FlowDirection), Order=1007)]
        private FontStretch _fontStretch = FontStretch.Normal;
        
        [DataMember(Name = nameof(FlowDirection), Order=1008)]
        private double _pixelsPerDip = 1.0;
        
        [DataMember(Name = nameof(FillBrush), Order = 2001)]
        private MapBrush _fillBrush = new();

        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                if (_fontFamily == value) return;
                _fontFamily = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize == value) return;
                _fontSize = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            } 
        }

        public FontStyle FontStyle
        {
            get => _fontStyle;
            set
            {
                if (_fontStyle == value) return;
                _fontStyle = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            } 
        }

        public FontWeight FontWeight
        {
            get => _fontWeight;
            set
            {
                if (_fontWeight == value) return;
                _fontWeight = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        public FontStretch FontStretch
        {
            get => _fontStretch;
            set
            {
                if (_fontStretch == value) return;
                _fontStretch = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        public FlowDirection FlowDirection
        {
            get => _flowDirection;
            set
            {
                if (_flowDirection == value) return;
                _flowDirection = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        public double PixelsPerDip
        {
            get => _pixelsPerDip;
            set
            {
                if (_pixelsPerDip == value) return;
                _pixelsPerDip = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        public MapBrush FillBrush
        {
            get => _fillBrush;
            set
            {
                if (Equals(value, _fillBrush)) return;
                _fillBrush.PropertyChanged -= OnBrushChanged;
                _fillBrush = value;
                _fillBrush.PropertyChanged += OnBrushChanged;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        public override Brush GetRenderBrush()
        {
            var formattedText = new FormattedText(
                Text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection,
                new Typeface(
                    new FontFamily(FontFamily),
                    (System.Windows.FontStyle)FontStyleConverter.Convert(FontStyle, typeof(System.Windows.FontStyle), null, null),
                    (System.Windows.FontWeight)FontWeightConverter.Convert(FontWeight, typeof(System.Windows.FontWeight), null, null),
                    (System.Windows.FontStretch)FontStretchConverter.Convert(FontStretch, typeof(System.Windows.FontStretch), null, null)
                ),
                FontSize,
                Brushes.Black,// This brush does not matter since we use the geometry of the text.
                PixelsPerDip 
            );
            var geometry = formattedText.BuildGeometry(new Point());
            if (geometry.CanFreeze)
                geometry.Freeze();
            return new DrawingBrush(new GeometryDrawing(FillBrush.GetRenderBrush(),
                new Pen(), geometry));
        }
        
        private void OnBrushChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == FillBrush)
                OnPropertyChanged(nameof(FillBrush));
            if (e.PropertyName == nameof(IRendersBrush.RenderedBrush)) OnRenderBrushUpdated();
        }
        
        public override string ToString()
        {
            return $"Text ({Text})";
        }
    }
}