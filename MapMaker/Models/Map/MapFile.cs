using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using MapMaker.Models.Library;
using MonitoredUndo;

namespace MapMaker.Models.Map
{
    [DataContract]
    public class MapFile : SmartObject, ISupportsUndo
    {
        [DataMember(Name = nameof(Name), Order = 0)]
        private string _name = "UntitledMap1";
        
        [DataMember(Name = nameof(PixelHeight), Order=1)]
        private int _pixelHeight = 3500;
        
        [DataMember(Name=nameof(PixelWidth), Order=2)]
        private int _pixelWidth = 3500;
        
        [DataMember(Name=nameof(ExportGrid), Order=1001)]
        private bool _exportGrid;
        
        [DataMember(Name=nameof(ImageFiles), Order=2001)]
        private SmartCollection<LibraryImage> _imageFiles = new();

        [DataMember(Name=nameof(ImageFiles), Order=3001)]
        private SmartCollection<MapBrush> _brushes = new();

        [DataMember(Name = nameof(FontFamily), Order = 4001)]
        private ObservableCollection<string> _fonts = new();
        
        [DataMember(Name=nameof(Layers), Order=int.MaxValue)]
        private SmartCollection<MapLayer> _layers = new();

        


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
        
        public bool ExportGrid
        {
            get => _exportGrid;
            set
            {
                if (value == _exportGrid) return;
                _exportGrid = value;
                OnPropertyChanged();
            }
        }
        
        public int PixelWidth
        {
            get => _pixelWidth;
            set
            {
                if (value == _pixelWidth) return;
                _pixelWidth = value;
                OnPropertyChanged();
            }
        }
        
        public int PixelHeight
        {
            get => _pixelHeight;
            set
            {
                if (value == _pixelHeight) return;
                _pixelHeight = value;
                OnPropertyChanged();
            }
        }
        
        public SmartCollection<MapLayer> Layers
        {
            get => _layers;
            set
            {
                if (Equals(value, _layers)) return;
                _layers = value;
                OnPropertyChanged();
            }
        }

        public SmartCollection<LibraryImage> ImageFiles
        {
            get => _imageFiles;
            set
            {
                if (Equals(value, _imageFiles)) return;
                _imageFiles = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Fonts
        {
            get => _fonts;
            set
            {
                if (_fonts == value) return;
                _fonts = value;
                OnPropertyChanged();
            }
        }

        public SmartCollection<MapBrush> Brushes
        {
            get => _brushes;
            set
            {
                if (_brushes == value) return;
                _brushes = value;
                OnPropertyChanged();
            }
        }

        public object GetUndoRoot()
        {
            return this;
        }

        protected override void OnClone(object clone)
        {
            base.OnClone(clone);

            var myClone = (MapFile) clone;
            myClone._layers = (SmartCollection<MapLayer>) _layers.Clone();
            myClone._imageFiles = (SmartCollection<LibraryImage>) _imageFiles.Clone();
            myClone._brushes = (SmartCollection<MapBrush>) _brushes.Clone();
            myClone._fonts = new ObservableCollection<string>(_fonts);
        }
    }
}