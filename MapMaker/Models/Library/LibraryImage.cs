using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace MapMaker.Models.Library
{
    public class LibraryImage : SmartObject, IRendersBrush
    {
        private BitmapImage? _bitmap;
        private string? _fileExtension;
        private long _fileSize;
        private string? _fullName;
        private Guid _id = Guid.NewGuid();
        private string? _path;
        private int _pixelHeight;
        private int _pixelWidth;
        private Brush? _renderBrush;
        private string? _shortName;

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
            get => _fullName!;
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
            get => _shortName!;
            set
            {
                if (value == _shortName) return;
                _shortName = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public string Path
        {
            get => _path?? string.Empty;
            set
            {
                if (value == _path) return;
                _path = value;
                OnPropertyChanged();
                _bitmap = null;
                OnPropertyChanged(nameof(Bitmap));
                _renderBrush = null;
                OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
            }
        }

        [XmlAttribute]
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

        [XmlAttribute]
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


        [XmlAttribute]
        public string FileExtension
        {
            get => _fileExtension ?? string.Empty;
            set
            {
                if (value == _fileExtension) return;
                _fileExtension = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public long FileSize
        {
            get => _fileSize;
            set
            {
                if (value == _fileSize) return;
                _fileSize = value;
                OnPropertyChanged();
            }
        }


        [XmlIgnore]
        [IgnoreDataMember]
        [NotMapped]
        public BitmapImage Bitmap
        {
            get
            {
                if (_bitmap == null)
                    try
                    {
                        _bitmap = new BitmapImage(new Uri(Path))
                            {CacheOption = BitmapCacheOption.OnLoad, DecodePixelWidth = 70};
                        _bitmap.Freeze();
                        OnPropertyChanged();
                        return _bitmap;
                    }
                    finally
                    {
                        DispatchNotifications();
                    }

                return _bitmap;
            }
            set
            {
                if (Equals(value, _bitmap)) return;
                _bitmap = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IRendersBrush.RenderedBrush));
            }
        }

        public Brush RenderedBrush => _renderBrush ??= GetRenderBrush();


        public Brush GetRenderBrush()
        {
            return new ImageBrush(Bitmap);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _bitmap = null;
            base.Dispose(disposing);
        }
    }
}