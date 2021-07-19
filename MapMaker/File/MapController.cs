using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using MapMaker.Annotations;
using MapMaker.Properties;

namespace MapMaker.File
{
    public class MapController : INotifyPropertyChanged
    {
        private MapFile _mapFile;
        private double _scale = 0.5;
        private Point _offset;
        private MapLayer _selectedLayer;
        private MapObject? _selectedObject;
        
        private Tool _selectedTool;
        private readonly IList<Tool> _tools;
        public event PropertyChangedEventHandler PropertyChanged;

        public MapController()
        {
            MapFile = new MapFile();

            SelectedTool = new Pointer(this);
            _tools = new List<Tool>
            {
                SelectedTool,
                new Pan(this)
            };
        }

        public IEnumerable<Tool> Tools => _tools;

        public Tool SelectedTool
        {
            get => _selectedTool;
            set
            {
                if (value == _selectedTool) return;
                _selectedTool = value;
                OnPropertyChanged();
            }
        }

        

        public MapObject? SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (Equals(value, _selectedObject)) return;
                _selectedObject = value;
                OnPropertyChanged();
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                if (value.Equals(_scale)) return;
                _scale = value;
                OnPropertyChanged();
            }
        }

        public Point Offset
        {
            get => _offset;
            set
            {
                if (value == _offset) return;
                _offset = value;
                OnPropertyChanged();
            }
        }


        public MapFile MapFile
        {
            get => _mapFile;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (Equals(value, _mapFile)) return;
                _mapFile = value;
                OnPropertyChanged();
                SelectedLayer = value.Layers[0];
            }
        }

        public MapLayer SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (Equals(value, _selectedLayer)) return;
                _selectedLayer = value;
                OnPropertyChanged();
            }
        }

        public void AddObject(MapObject newObject)
        {
            if (newObject is MapImage newImage)
            {
                var linkedBitmap = MapFile.ImageFiles.SingleOrDefault(f => f.Id == newImage.Image.Id);
                if (linkedBitmap == null)
                {
                    linkedBitmap = newImage.Image;
                    MapFile.ImageFiles.Add(linkedBitmap);
                }

                newImage.Image = linkedBitmap;
            }
            
            SelectedLayer.MapObjects.Add(newObject);
        }

        public Point SnapToGrid(Point input)
        {
            return Settings.Default.ShowGrid && Settings.Default.SnapToGrid 
                ? ClosestGridPoint(input) 
                : input;
        }
        
        public Point ClosestGridPoint(Point input)
        {
            var w = Settings.Default.GridCellWidth;
            var modX = input.X % w;
            var modY = input.Y % w;
            var baseX = input.X - modX;
            var baseY = input.Y - modY;
            return new Point(
                modX / w > 0.5 ? baseX + w : baseX, 
                modY / w > 0.5 ? baseY + w : baseY);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}