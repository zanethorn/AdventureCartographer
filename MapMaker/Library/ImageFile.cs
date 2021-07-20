using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using MapMaker.Annotations;

namespace MapMaker.Library
{
    public class ImageFile:INotifyPropertyChanged
    {
        private Guid _id = Guid.NewGuid();
        private string _fullName;
        private string _shortName;
        private BitmapImage? _bitmap;
        public event PropertyChangedEventHandler? PropertyChanged;

        [XmlAttribute]
        public Guid Id
        {
            get => _id;
            set
            {
                if (value.Equals(_id)) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public string FullName
        {
            get => _fullName;
            set
            {
                if (value == _fullName) return;
                _fullName = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public string ShortName
        {
            get => _shortName;
            set
            {
                if (value == _shortName) return;
                _shortName = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string Path { get; set; }
        
        [XmlAttribute]
        public int PixelWidth { get; set; }
        
        [XmlAttribute]
        public int PixelHeight { get; set; }
        
        
        [XmlAttribute]
        public string FileExtension { get; set; }
        
        [XmlAttribute]
        public long FileSize { get; set; }

        [XmlIgnore]
        public IList<ImageTags> Tags { get; set; } = new List<ImageTags>();


        [XmlIgnore]
        [IgnoreDataMember]
        [NotMapped]
        public BitmapImage Bitmap
        {
            get
            {
                if (_bitmap == null)
                {
                    Task.Run(() =>
                    {
                        _bitmap = new BitmapImage(new Uri(Path))
                            {CacheOption = BitmapCacheOption.OnDemand, DecodePixelWidth = 70};
                        _bitmap.Freeze();
                        if (Dispatcher.CurrentDispatcher.CheckAccess())
                        {
                            OnPropertyChanged();
                        }
                        else
                        {
                            Dispatcher.CurrentDispatcher.BeginInvoke(() => OnPropertyChanged());
                        }
                    });
                }

                return _bitmap;
            }
            set
            {
                if (Equals(value, _bitmap)) return;
                _bitmap = value;
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