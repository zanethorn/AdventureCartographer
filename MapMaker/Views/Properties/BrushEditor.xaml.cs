using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MapMaker.Models.Library;
using MapMaker.Models.Map;

namespace MapMaker.Views.Panels
{
    public partial class BrushEditor : UserControl
    {
        public static readonly DependencyProperty SelectedColorStopProperty =
            DependencyProperty.Register(nameof(SelectedColorStop), typeof(GradientColorStop), typeof(BrushEditor));

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register(nameof(Brush),typeof(MapBrush), typeof(BrushEditor),
                new PropertyMetadata(OnBrushChangedCallback));
        
        private static void OnBrushChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var me = (BrushEditor) sender;
            if (e.Property == BrushProperty)
            {
                me.SelectedColorStop = me.Brush.Colors[0];
                if (me._owner == null || me._owner.Brush == me.Brush) return;
                me._owner.Brush = me.Brush;
            }
        }
        
        public static BrushEditor Instance { get; private set; }


        private BrushButton? _owner;

        public BrushEditor()
        {
            Instance = this;
            InitializeComponent();
            ColorCanvas.SelectedColorChanged += OnSelectedColorChanged;
        }

        public GradientColorStop SelectedColorStop
        {
            get => (GradientColorStop) GetValue(SelectedColorStopProperty);
            set => SetValue(SelectedColorStopProperty, value);
        }

        public MapBrush Brush
        {
            get => (MapBrush) GetValue(BrushProperty);
            set
            {
                SetValue(BrushProperty, value);
                
            } 
        }

        public void TakeControl(BrushButton owner)
        {
            _owner = owner;
            Brush = _owner.Brush;
        }
        

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
                Models.Map.BrushTypes.LinearGradient => Visibility.Visible,
                Models.Map.BrushTypes.RadialGradient => Visibility.Visible,
                _ => Visibility.Collapsed
            };

            ColorCanvas.Visibility = Brush.BrushType switch
            {
                Models.Map.BrushTypes.Solid => Visibility.Visible,
                Models.Map.BrushTypes.LinearGradient => Visibility.Visible,
                Models.Map.BrushTypes.RadialGradient => Visibility.Visible,
                _ => Visibility.Collapsed
            };
            
            ImageFileView.Visibility = Brush.BrushType switch
            {
                Models.Map.BrushTypes.Image => Visibility.Visible,
                _ => Visibility.Collapsed
            };
        }

        

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(LibraryImage)))
            {
                var data = e.Data.GetData(typeof(LibraryImage));
                if (data is LibraryImage imgFile)
                {
                    Brush.BrushType = Models.Map.BrushTypes.Image;
                    Brush.NestedBrushRenderer = imgFile;
                }
            }
        }
    }
}