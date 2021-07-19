using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using MapMaker.Annotations;
using MapMaker.Library;

namespace MapMaker.File
{
    public class MapFile: INotifyPropertyChanged
    {
        private string _name = "UntitledMap1";
        private int _pixelWidth=1750;
        private int _pixelHeight=1750;

        private ObservableCollection<MapLayer> _layers = new()
        {
            new MapLayer
            {
                Name="Untitled_Layer_1"
            }
        };

        
        public event PropertyChangedEventHandler PropertyChanged;

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


        public ObservableCollection<ImageFile> ImageFiles { get; set; } = new();
        
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}