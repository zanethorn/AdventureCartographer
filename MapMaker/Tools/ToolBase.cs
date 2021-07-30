using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MapMaker.Controllers;
using MapMaker.Models;

namespace MapMaker.Tools
{
    public abstract class ToolBase : SmartObject, ITool
    {
        private Point _lastPosition;
        private bool _isDown;

        private SettingsController? _settingsController;
        private EditorController? _editorController;
        private MapController? _mapController;
        private Cursor _cursor = Cursors.Arrow;

        protected ToolBase(string iconName)
        {
            Icon = (BitmapImage)App.Current.FindResource(iconName);
        }

        protected SettingsController SettingsController =>
            _settingsController ??= (SettingsController) App.Current.FindResource(nameof(SettingsController));

        protected EditorController EditorController =>
            _editorController ??= (EditorController) App.Current.FindResource(nameof(EditorController));

        protected MapController MapController =>
            _mapController ??= (MapController) App.Current.FindResource(nameof(MapController));

        protected bool IsDown => _isDown;
        
        public BitmapImage Icon { get; }
        public Point Position => _lastPosition;

        public Cursor Cursor
        {
            get => _cursor;
            protected set
            {
                if (_cursor == value) return;
                _cursor = value;
                OnPropertyChanged();
            } 
        }

        public void Up(Point position)
        {
            _isDown = false;
            OnUp(position);
            _lastPosition = position;
        }

        public void Down(Point position)
        {
            _isDown = true;
            OnDown(position);
            _lastPosition = position;
        }

        public void Move(Point position)
        {
            OnMove(position);
            _lastPosition = position;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        protected abstract void OnUp(Point position);
        protected abstract void OnDown(Point position);

        protected abstract void OnMove(Point position);
    }
}