using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MapMaker.Annotations;
using MapMaker.Library;

namespace MapMaker.File
{
    public class MapFile: INotifyPropertyChanged
    {
        private string _name;
        private int _cellsWide = 25;
        private int _cellsHigh = 25;
        private int _cellDisplayWidth = 70;
        private int _cellDisplayHeight = 70;
        private ObservableCollection<MapLayer> _layers;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public int PixelWidth => CellsWide * CellDisplayWidth;

        public int PixelHeight => CellsHigh * CellDisplayHeight;
        
        public int CellsWide
        {
            get => _cellsWide;
            set
            {
                if (value == _cellsWide) return;
                _cellsWide = value;
                OnPropertyChanged();
            }
        }

        public int CellsHigh
        {
            get => _cellsHigh;
            set
            {
                if (value == _cellsHigh) return;
                _cellsHigh = value;
                OnPropertyChanged();
            }
        }

        public int CellDisplayWidth
        {
            get => _cellDisplayWidth;
            set
            {
                if (value == _cellDisplayWidth) return;
                _cellDisplayWidth = value;
                OnPropertyChanged();
            }
        }

        public int CellDisplayHeight
        {
            get => _cellDisplayHeight;
            set
            {
                if (value == _cellDisplayHeight) return;
                _cellDisplayHeight = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MapLayer> Layers
        {
            get => _layers;
            set
            {
                if (Equals(value, _layers)) return;
                _layers = value;
                OnPropertyChanged();
            }
        }

        public IList<ImageFile> SourceFiles { get; set; } = new List<ImageFile>();
        
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}