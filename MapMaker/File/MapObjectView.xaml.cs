﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using MapMaker.Converters;
using MapMaker.Properties;
using Microsoft.EntityFrameworkCore;

namespace MapMaker.File
{
    public partial class MapObjectView : UserControl
    {
        private static readonly BrushConverter _brushConverter = new();
        
        private bool _scaleX;
        private bool _scaleY;
        private bool _offsetX;
        private bool _offsetY;


        public MapObjectView()
        {
            InitializeComponent();
            Controller = (MapController) FindResource(nameof(MapController));

            
        }

        public MapController Controller { get; }

        public MapObject MapObject => (MapObject) DataContext;
        
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var binding = new Binding(nameof(MapController.SelectedObject))
            {
                Source = Controller,
                ConverterParameter = MapObject,
                Converter = (IValueConverter) FindResource(nameof(EqualsIsVisibilityConverter))
            };


            ThumbNW.SetBinding(Thumb.VisibilityProperty, binding);
            ThumbN.SetBinding(Thumb.VisibilityProperty, binding);
            ThumbNE.SetBinding(Thumb.VisibilityProperty, binding);
            ThumbW.SetBinding(Thumb.VisibilityProperty, binding);
            ThumbE.SetBinding(Thumb.VisibilityProperty, binding);
            ThumbSW.SetBinding(Thumb.VisibilityProperty, binding);
            ThumbS.SetBinding(Thumb.VisibilityProperty, binding);
            ThumbSE.SetBinding(Thumb.VisibilityProperty, binding);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Controller.SelectObject(MapObject);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Controller.SelectObject(null);
        }
        
        private void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            Controller.SelectObject(MapObject, true);
        }

        private void OnDragStartedNW(object sender, DragStartedEventArgs e)
        {
            _scaleX = true;
            _scaleY = true;
            _offsetX = true;
            _offsetY = true;
        }

        private void OnDragStartedN(object sender, DragStartedEventArgs e)
        {
            _scaleY = true;
            _offsetY = true;
        }

        private void OnDragStartedNE(object sender, DragStartedEventArgs e)
        {
            _scaleX = true;
            _scaleY = true;
            _offsetY = true;
        }

        private void OnDragStartedW(object sender, DragStartedEventArgs e)
        {
            _scaleX = true;
            _offsetX = true;
        }

        private void OnDragStartedE(object sender, DragStartedEventArgs e)
        {
            _scaleX = true;
        }

        private void OnDragStartedSW(object sender, DragStartedEventArgs e)
        {
            _scaleX = true;
            _scaleY = true;
            _offsetX = true;
        }

        private void OnDragStartedS(object sender, DragStartedEventArgs e)
        {
            _scaleY = true;
        }

        private void OnDragStartedSE(object sender, DragStartedEventArgs e)
        {
            _scaleX = true;
            _scaleY = true;
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var x = e.HorizontalChange;
            var y = e.VerticalChange;

            if (_offsetX)
            {
                x = -x;
            }

            if (_offsetY)
            {
                y = -y;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (Math.Abs(x) > Math.Abs(y))
                {
                    y = x;
                }
                else
                {
                    x = y;
                }
            }

            if (_scaleX)
            {
                var newX = MapObject.PixelWidth + x;
                if (newX < 1.0)
                    newX = 1.0;
                MapObject.PixelWidth = newX;
            }

            if (_scaleY)
            {
                var newY = MapObject.PixelHeight + y;
                if (newY < 1.0)
                    y = 1.0;
                MapObject.PixelHeight = newY;
            }

            if (!_offsetX)
            {
                x = 0;
            }

            if (!_offsetY)
            {
                y = 0;
            }

            MapObject.Offset = new Point(MapObject.Offset.X - x, MapObject.Offset.Y - y);
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _scaleX = false;
            _scaleY = false;
            _offsetX = false;
            _offsetY = false;

            MapObject.Offset = Controller.SnapToGrid(MapObject.Offset);
            var sizer = Controller.SnapToGrid(new Point(MapObject.PixelWidth, MapObject.PixelHeight));
            MapObject.PixelWidth = sizer.X;
            MapObject.PixelHeight = sizer.Y;
        }


        
    }
}