using System.ComponentModel;
using MonitoredUndo;

namespace MapMaker.Models
{
    public class Settings : SmartObject,ISupportsUndo
    {
        private int _gridCellWidth = 140;
        private float _gridPenWidth=0.05F;
        private string _gridPenColor="#FF00FFFF";
        private string _backgroundGridColor1 ="#FF666666";
        private string _backgroundGridColor2="#FFCCCCCC";
        private bool _showGrid = true;
        private bool _snapToGrid=true;
        private string _controlHighlightColor = "#FFFFFF00";
        private string _defaultLibraryName = "default";

        public string DefaultLibraryName
        {
            get => _defaultLibraryName;
            set
            {
                if (_defaultLibraryName == value) return;
                _defaultLibraryName = value;
                OnPropertyChanged();
            }
        }


        public int GridCellWidth
        {
            get => _gridCellWidth;
            set
            {
                if (value == _gridCellWidth) return;
                _gridCellWidth = value;
                OnPropertyChanged();
            }
        }


        public float GridPenWidth
        {
            get => _gridPenWidth;
            set
            {
                if (value == _gridPenWidth) return;
                _gridPenWidth = value;
                OnPropertyChanged();
            }
        }

        public string GridPenColor
        {
            get => _gridPenColor;
            set
            {
                if (value == _gridPenColor) return;
                _gridPenColor = value;
                OnPropertyChanged();
            }
        }

        public string BackgroundGridColor1
        {
            get => _backgroundGridColor1;
            set
            {
                if (value == _backgroundGridColor1) return;
                _backgroundGridColor1 = value;
                OnPropertyChanged();
            }
        }

        public string BackgroundGridColor2
        {
            get => _backgroundGridColor2;
            set
            {
                if (value == _backgroundGridColor2) return;
                _backgroundGridColor2 = value;
                OnPropertyChanged();
            }
        }


        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                if (value == _showGrid) return;
                _showGrid = value;
                OnPropertyChanged();
            }
        }


        public bool SnapToGrid
        {
            get => _snapToGrid;
            set
            {
                if (value == _snapToGrid) return;
                _snapToGrid = value;
                OnPropertyChanged();
            }
        }


        public string ControlHighlightColor
        {
            get => _controlHighlightColor;
            set
            {
                if (value == _controlHighlightColor) return;
                _controlHighlightColor = value;
                OnPropertyChanged();
            } 
        }

        public object GetUndoRoot()
        {
            return this;
        }
    }
}