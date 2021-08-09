using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MapMaker.Models;
using MapMaker.Models.Map;
using MapMaker.Tools;
using MapMaker.Views.Editor;
using MonitoredUndo;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace MapMaker.Controllers
{
    public class EditorController : SmartObject
    {
        private ITool _selectedTool;
        private MapController? _mapController;
        private Point _offset = new Point(-760,-760);

        private double _scale = 0.5;
        private MapBrush? _selectedBrush;
        private MapLayer? _selectedLayer;
        private MapFile? _selectedMap;
        private MapObject? _selectedObject;
        
        private ToolTrayPanels _selectedToolTray;
        private SettingsController? _settings;
        private MapBrush _defaultBackgroundBrush = new (Colors.Beige);
        private MapBrush _defaultForegroundBrush = new (Colors.Blue);

        private string? _lastUndoCaller;
        private UndoBatch? _currentUndoBatch;
        private List<ITool> _tools;

        public EditorController()
        {
            _selectedBrush = _defaultBackgroundBrush;
            _selectedTool = new Pointer();
            _tools = new List<ITool>()
            {
                _selectedTool,
                new Pan(),
                new Shape(),
                new Text()
            };
        }

        private SettingsController Settings =>
            _settings ??= (SettingsController) Application.Current.FindResource(nameof(SettingsController))!;

        private MapController MapController =>
            _mapController ??= (MapController) Application.Current.FindResource(nameof(MapController))!;

        public bool CanUndo => UndoService.Current[SelectedMap].CanUndo;
        public bool CanRedo => UndoService.Current[SelectedMap].CanRedo;

        public IEnumerable<ChangeSet> UndoHistory => UndoService.Current[SelectedMap].UndoStack;

        public IEnumerable<ITool> Tools => _tools;

        public ITool SelectedTool
        {
            get => _selectedTool;
            set
            {
                if (value == _selectedTool) return;
                _selectedTool = value;
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


        public ToolTrayPanels SelectedToolTray
        {
            get => _selectedToolTray;
            set
            {
                if (value == _selectedToolTray) return;
                _selectedToolTray = value;
                OnPropertyChanged();
            }
        }

        public MapFile SelectedMap
        {
            get => _selectedMap!;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (Equals(value, _selectedMap)) return;
                _selectedMap = value;
                OnPropertyChanged();
                SelectedLayer = value.Layers[0];
                
                UndoService.Current[value].UndoStackChanged+= delegate
                {
                    OnUpdateUndoStack();
                };
                UndoService.Current[value].RedoStackChanged+= delegate
                {
                    OnUpdateUndoStack();
                };
                OnUpdateUndoStack();
            }
        }

       

        public MapLayer SelectedLayer
        {
            get => _selectedLayer!;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (Equals(value, _selectedLayer)) return;
                _selectedLayer = value;
                OnPropertyChanged();
                SelectedObject = null;
            }
        }

        public MapObject? SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (Equals(value, _selectedObject)) return;
                _selectedObject = value;
                EndUndo();
                OnPropertyChanged();
            }
        }

        public MapBrush? SelectedBrush
        {
            get => _selectedBrush;
            set
            {
                if (Equals(value, _selectedBrush)) return;
                _selectedBrush = value;
                OnPropertyChanged();
            }
        }

        public MapBrush DefaultBackgroundBrush
        {
            get => _defaultBackgroundBrush;
            set
            {
                if (_defaultBackgroundBrush == value) return;
                _defaultBackgroundBrush = value;
                OnPropertyChanged();
            } 
        }

        public MapBrush DefaultForegroundBrush
        {
            get => _defaultForegroundBrush;
            set
            {
                if (_defaultForegroundBrush == value) return;
                _defaultForegroundBrush = value;
                OnPropertyChanged();
            }
        }

        public Task Init()
        {
            SelectedMap = MapController.NewMap();
            return Task.CompletedTask;
        }

        public void Undo()
        {
            if (CanUndo)
            {
                _currentUndoBatch?.Dispose();
                _currentUndoBatch = null;
                UndoService.Current[SelectedMap].Undo();
                OnUpdateUndoStack();
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                _currentUndoBatch?.Dispose();
                _currentUndoBatch = null;
                UndoService.Current[SelectedMap].Redo();
                OnUpdateUndoStack();
            }
        }

        public void BeginUndo(string description, [CallerMemberName] string? callerName = null)
        {
            BeginUndo(description, false, callerName);
        }

        public void BeginUndo(string description, bool startNewBatch, [CallerMemberName] string? propertyName = null)
        {
            if (_lastUndoCaller != propertyName)
                startNewBatch = true;
            _lastUndoCaller = propertyName;
            if (!startNewBatch) return;
            
            _currentUndoBatch?.Dispose();
            _currentUndoBatch = new UndoBatch(SelectedMap, description, true);
            OnUpdateUndoStack();
        }

        public void EndUndo()
        {
            _currentUndoBatch?.Dispose();
            _currentUndoBatch = null;
            OnUpdateUndoStack();
        }

        public Size SnapToGrid(Size input)
        {
            return Settings.Settings.ShowGrid && Settings.Settings.SnapToGrid
                ? GetClosestGridPoint(input)
                : input;
        }

        public Point SnapToGrid(Point input)
        {
            return Settings.Settings.ShowGrid && Settings.Settings.SnapToGrid
                ? GetClosestGridPoint(input)
                : input;
        }

        private Size GetClosestGridPoint(Size input)
        {
            var w = Settings.Settings.GridCellWidth;
            var modX = input.Width % w;
            var modY = input.Height % w;
            var baseX = input.Width - modX;
            var baseY = input.Height - modY;
            return new Size(
                modX / w > 0.5 ? baseX + w : baseX,
                modY / w > 0.5 ? baseY + w : baseY);
        }

        private Point GetClosestGridPoint(Point input)
        {
            var w = Settings.Settings.GridCellWidth;
            var modX = input.X % w;
            var modY = input.Y % w;
            var baseX = input.X - modX;
            var baseY = input.Y - modY;
            return new Point(
                modX / w > 0.5 ? baseX + w : baseX,
                modY / w > 0.5 ? baseY + w : baseY);
        }
        
        private void OnUpdateUndoStack()
        {
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            OnPropertyChanged(nameof(UndoHistory));
            DispatchNotifications();
        }

        public object GetUndoRoot()
        {
            return this;
        }
    }
}