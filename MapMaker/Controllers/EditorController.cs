using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MapMaker.Models;
using MapMaker.Models.Map;
using MonitoredUndo;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace MapMaker.Controllers
{
    public class EditorController : SmartObject
    {
        private Point _cursorPosition;
        private MapController? _mapController;
        private Point _offset = new Point(-760,-760);

        private double _scale = 0.5;
        private MapBrush? _selectedBrush;
        private MapLayer? _selectedLayer;
        private MapFile? _selectedMap;
        private MapObject? _selectedObject;

        private ToolTypes _selectedTool;
        private ToolTrayPanels _selectedToolTray;
        private SettingsController? _settings;
        private MapBrush _defaultBackgroundBrush = new MapBrush(Colors.Beige);
        private MapBrush _defaultForegroundBrush = new MapBrush(Colors.Blue);


        private SettingsController Settings =>
            _settings ??= (SettingsController) Application.Current.FindResource(nameof(SettingsController))!;

        private MapController MapController =>
            _mapController ??= (MapController) Application.Current.FindResource(nameof(MapController))!;

        public bool CanUndo => UndoService.Current[SelectedMap].CanUndo;
        public bool CanRedo => UndoService.Current[SelectedMap].CanRedo;

        public Cursor Cursor => SelectedTool switch
        {
            ToolTypes.Pointer => Cursors.Arrow,
            ToolTypes.Pan => Cursors.Hand,
            ToolTypes.Shape => Cursors.Arrow,
            _ => throw new InvalidEnumArgumentException()
        };

        public ToolTypes SelectedTool
        {
            get => _selectedTool;
            set
            {
                if (value == _selectedTool) return;
                _selectedTool = value;
                OnPropertyChanged();
            }
        }

        public Point CursorPosition
        {
            get => _cursorPosition;
            set
            {
                if (value.Equals(_cursorPosition)) return;
                _cursorPosition = value;
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
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
                DispatchNotifications();
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
            set => _defaultBackgroundBrush = value;
        }

        public MapBrush DefaultForegroundBrush
        {
            get => _defaultForegroundBrush;
            set => _defaultForegroundBrush = value;
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
                UndoService.Current[SelectedMap].Undo();

                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
                DispatchNotifications();
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                UndoService.Current[SelectedMap].Redo();

                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
                DispatchNotifications();
            }
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

        public object GetUndoRoot()
        {
            return this;
        }
    }
}