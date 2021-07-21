using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using MapMaker.Annotations;
using MapMaker.Library;

namespace MapMaker.File
{
    public class MapFile: SmartObject
    {
        private string _name = "UntitledMap1";
        private int _pixelWidth=1750;
        private int _pixelHeight=1750;

        private ObservableCollection<MapLayer> _layers = new();
        private ObservableCollection<ImageFile> _imageFiles = new();
        private bool _exportGrid;


        [XmlAttribute]
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

        [XmlAttribute]
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

        [XmlIgnore]
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


        [XmlIgnore]
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


        [XmlArray(nameof(Layers))]
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

        public ObservableCollection<ImageFile> ImageFiles
        {
            get => _imageFiles;
            set
            {
                if (Equals(value, _imageFiles)) return;
                _imageFiles = value;
                OnPropertyChanged();
            }
        }

        protected override void OnClone(object clone)
        {
            base.OnClone(clone);

            var myClone = (MapFile)clone;
            myClone._layers = new ObservableCollection<MapLayer>(_layers.Clone());
            myClone._imageFiles = new ObservableCollection<ImageFile>(_imageFiles.Clone());
        }
    }
}