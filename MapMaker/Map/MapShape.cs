using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MapMaker.Map
{
    public class MapShape:MapObject
    {
        private ShapeTypes _type;
        private MapBrush _fillBrush = new ();
        private MapBrush _strokeBrush = new ();
        private double _strokeThickness = 0.01;
        private int _sides = 3;
        private double _eccentricity = 0.5;

        public MapShape()
        {
            FillBrush = new MapBrush(Colors.Beige);
            StrokeBrush = new MapBrush(Colors.Blue);
        }

        [XmlAttribute]
        public ShapeTypes Type
        {
            get => _type;
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        [XmlAttribute]
        public int Sides
        {
            get => _sides;
            set
            {
                if (value == _sides) return;
                _sides = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        [XmlAttribute]
        public double Eccentricity
        {
            get => _eccentricity;
            set
            {
                if (value.Equals(_eccentricity)) return;
                _eccentricity = value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        [XmlElement]
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

        [XmlElement]
        public MapBrush StrokeBrush
        {
            get => _strokeBrush;
            set
            {
                if (Equals(value, _strokeBrush)) return;
                _strokeBrush.PropertyChanged -= OnBrushChanged;
                _strokeBrush = value;
                _strokeBrush.PropertyChanged += OnBrushChanged;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        [XmlAttribute]
        public double StrokeThickness
        {
            get => _strokeThickness;
            set
            {
                if (value.Equals(_strokeThickness)) return;
                _strokeThickness =  value;
                OnPropertyChanged();
                OnRenderBrushUpdated();
            }
        }

        public override Brush GetRenderBrush()
        {
            Geometry geometry = Type switch
            {
                ShapeTypes.Rectangle => new RectangleGeometry(new Rect(-1,-1,2,2)),
                ShapeTypes.Ellipse => new EllipseGeometry(new Point(0,0), 1,1),
                ShapeTypes.Polygon => new PathGeometry(new []{ new PathFigure(new Point(1,0),GetPolygonPoints(Sides), true)}),
                ShapeTypes.Star => new PathGeometry(new []{ new PathFigure(new Point(1,0),GetStarPoints(Sides,Eccentricity), true)}),
                _ => throw new ArgumentOutOfRangeException()
            };
            if (geometry.CanFreeze)
                geometry.Freeze();
            return new DrawingBrush(new GeometryDrawing(FillBrush.GetRenderBrush(), new Pen(StrokeBrush.GetRenderBrush(), _strokeThickness) ,geometry));
        }

        public override string ToString()
        {
            return $"Shape ({Type})";
        }

        private void OnBrushChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == FillBrush)
            {
                OnPropertyChanged(nameof(FillBrush));
            }
            else if (sender == StrokeBrush)
            {
                OnPropertyChanged(nameof(StrokeBrush));
            }
            if (e.PropertyName == nameof(IRendersBrush.RenderedBrush))
            {
                OnRenderBrushUpdated();
            }
        }

        private static IEnumerable<PathSegment> GetPolygonPoints(int sides)
        {
            for (var i = 1; i < sides; i++)
            {
                var t = i * 2.0 * Math.PI / sides;
                var x = Math.Cos(t);
                var y = Math.Sin(t);
                yield return new LineSegment(new Point(x, y), true);
            }
        }
        
        private static IEnumerable<PathSegment> GetStarPoints(int sides, double eccentricity)
        {
            var t = Math.PI / sides;
            var x = eccentricity * Math.Cos(t);
            var y = eccentricity * Math.Sin(t);
            yield return new LineSegment(new Point(x, y), true);
            
            for (var i = 1; i < sides; i++)
            {
                var tPoint = i * 2.0 * Math.PI / sides;
                var xPoint = Math.Cos(tPoint);
                var yPoint = Math.Sin(tPoint);
                yield return new LineSegment(new Point(xPoint, yPoint), true);
                
                var tMid =(i+0.5) * 2.0 * Math.PI / sides;
                var xMid = eccentricity *  Math.Cos(tMid);
                var yMid = eccentricity * Math.Sin(tMid);
                yield return new LineSegment(new Point(xMid, yMid), true);
            }
        }
    }
}