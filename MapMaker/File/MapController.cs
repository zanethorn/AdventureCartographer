using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MapMaker.Annotations;

namespace MapMaker.File
{
    public class MapController : INotifyPropertyChanged
    {
        private MapFile _mapFile;
        private float _scale = 0.5f;
        private int _offsetX;
        private int _offsetY;
        private MapLayer _selectedLayer;
        private bool _isToolActive;
        private Tool _selectedTool;
        private IList<Tool> _tools;
        public event PropertyChangedEventHandler PropertyChanged;

        public MapController()
        {
            MapFile = new MapFile();

            SelectedTool = new Pointer(this);
            _tools = new List<Tool>
            {
                SelectedTool
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

        public bool IsToolActive
        {
            get => _isToolActive;
            set
            {
                if (value == _isToolActive) return;
                _isToolActive = value;
                OnPropertyChanged();
            }
        }

        public float Scale
        {
            get => _scale;
            set
            {
                if (value.Equals(_scale)) return;
                _scale = value;
                OnPropertyChanged();
            }
        }

        public int OffsetX
        {
            get => _offsetX;
            set
            {
                if (value == _offsetX) return;
                _offsetX = value;
                OnPropertyChanged();
            }
        }

        public int OffsetY
        {
            get => _offsetY;
            set
            {
                if (value == _offsetY) return;
                _offsetY = value;
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}