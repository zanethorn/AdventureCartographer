﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using MapMaker.Controllers;
using MapMaker.Converters;
using MapMaker.Models.Map;
using MonitoredUndo;

namespace MapMaker.Views
{
    public partial class MapObjectView : UserControl
    {
        private static readonly BrushConverter _brushConverter = new();
        private readonly EditorController _editorController;
        private readonly MapController _mapController;

        private readonly SettingsController _settingsController;
        private bool _offsetX;
        private bool _offsetY;

        private bool _scaleX;
        private bool _scaleY;


        public MapObjectView()
        {
            InitializeComponent();
            _settingsController = (SettingsController) FindResource(nameof(SettingsController));
            _editorController = (EditorController) FindResource(nameof(EditorController));
            _mapController = (MapController) FindResource(nameof(MapController));
            _mapController.PropertyChanged += OnControllerPropertyChanged;
        }

        public MapObject MapObject => (DataContext as MapObject)!;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var binding = new Binding(nameof(_editorController.SelectedObject))
            {
                Source = _editorController,
                ConverterParameter = MapObject,
                Converter = (IValueConverter) FindResource(nameof(EqualsIsVisibilityConverter))
            };


            ThumbNW.SetBinding(VisibilityProperty, binding);
            ThumbN.SetBinding(VisibilityProperty, binding);
            ThumbNE.SetBinding(VisibilityProperty, binding);
            ThumbW.SetBinding(VisibilityProperty, binding);
            ThumbE.SetBinding(VisibilityProperty, binding);
            ThumbSW.SetBinding(VisibilityProperty, binding);
            ThumbS.SetBinding(VisibilityProperty, binding);
            ThumbSE.SetBinding(VisibilityProperty, binding);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            
        }

        private void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            _editorController.SelectedObject = MapObject;
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

            var pixelWidth = MapObject.Size.Width;
            var pixelHeight = MapObject.Size.Height;

            if (_offsetX) x = -x;

            if (_offsetY) y = -y;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (Math.Abs(x) > Math.Abs(y))
                    y = x;
                else
                    x = y;
            }

            if (_scaleX)
            {
                var newX = pixelWidth + x;
                if (newX < 1.0)
                    newX = 1.0;
                pixelWidth = newX;
            }

            if (_scaleY)
            {
                var newY = pixelHeight + y;
                if (newY < 1.0)
                    y = 1.0;
                pixelHeight = newY;
            }

            if (!_offsetX) x = 0;

            if (!_offsetY) y = 0;


            using (new UndoBatch(_editorController.SelectedMap, $"Resize {MapObject}", true))
            {
                MapObject.Offset = new Point(MapObject.Offset.X - x, MapObject.Offset.Y - y);
                MapObject.Size = new Size(pixelWidth, pixelHeight);
            }
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _scaleX = false;
            _scaleY = false;
            _offsetX = false;
            _offsetY = false;
        }

        private void OnControllerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_editorController.SelectedObject))
            {
                if (_editorController.SelectedObject == MapObject)
                    ContentDisplay.Stroke =
                        (Brush) _brushConverter.ConvertFrom(_settingsController.Settings.ControlHighlightColor)!;
                else
                    ContentDisplay.Stroke = Brushes.Transparent;
            }
        }
    }
}