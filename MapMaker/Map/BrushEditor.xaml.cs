using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MapMaker.Map
{
    public partial class BrushEditor : UserControl
    {
        public static readonly DependencyProperty SelectedColorStopProperty =
            DependencyProperty.Register(nameof(SelectedColorStop), typeof(GradientColorStop), typeof(BrushEditor));


        public BrushEditor()
        {
            InitializeComponent();
            ColorCanvas.SelectedColorChanged += OnSelectedColorChanged;
            DataContextChanged += OnBrushChanged;
        }

        public GradientColorStop SelectedColorStop
        {
            get => (GradientColorStop) GetValue(SelectedColorStopProperty);
            set
            {
                SetValue(SelectedColorStopProperty, value);
                //ColorCanvas.SelectedColor = value.MediaColor;
            }
        }

        public MapBrush Brush => (MapBrush) DataContext;

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var thumb = (Thumb) sender;
            SelectedColorStop = (GradientColorStop) thumb.DataContext;
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb) sender;
            var colorStop = (GradientColorStop) thumb.DataContext;
            var offset = e.HorizontalChange / GradientEditor.ActualWidth;
            colorStop.Offset = Math.Clamp(colorStop.Offset + offset, 0.0, 1.0);
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var colorArray = (ICollection<GradientColorStop>) GradientEditor.ItemsSource;
            var pos = e.GetPosition(GradientEditor);
            var offset = pos.X / GradientEditor.ActualWidth;
            SelectedColorStop = new GradientColorStop {Offset = offset, MediaColor = Colors.Black};
            colorArray.Add(SelectedColorStop);
        }

        private void OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            SelectedColorStop.MediaColor = e.NewValue ?? SelectedColorStop.MediaColor;
        }

        private void OnBrushTypesChanged(object sender, SelectionChangedEventArgs e)
        {
            GradientEditor.Visibility = Brush.BrushType switch
            {
                Map.BrushTypes.LinearGradient => Visibility.Visible,
                Map.BrushTypes.RadialGradient => Visibility.Visible,
                _ => Visibility.Collapsed
            };
        }

        private void OnBrushChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DataContextProperty)
            {
                SelectedColorStop = Brush.Colors[0];
            }
        }
    }
}