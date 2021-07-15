﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MapMaker.Annotations;

namespace MapMaker.File
{
    public class MapController : INotifyPropertyChanged
    {
        private MapFile _mapFile;
        private double _scale = 0.5;
        private double _offsetX;
        private double _offsetY;
        private MapLayer _selectedLayer;
        private MapObject? _selectedObject;
        
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

        public double OffsetX
        {
            get => _offsetX;
            set
            {
                if (value == _offsetX) return;
                _offsetX = value;
                OnPropertyChanged();
            }
        }

        public double OffsetY
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