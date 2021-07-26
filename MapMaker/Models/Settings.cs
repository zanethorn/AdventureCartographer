using System.ComponentModel;

namespace MapMaker.Models
{
    public class Settings : SmartObject, INotifyPropertyChanged
    {
        private int _gridCellWidth = 140;

        public string DefaultLibraryName { get; set; }


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


        public float GridPenWidth { get; set; }

        public string GridPenColor { get; set; }

        public string BackgroundGridColor1 { get; set; }

        public string BackgroundGridColor2 { get; set; }


        public bool ShowGrid { get; set; }


        public bool SnapToGrid { get; set; }


        public string ControlHighlightColor { get; set; }
    }
}