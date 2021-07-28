using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace MapMaker.Models.Library
{
    [DataContract]
    public class LibraryImage : SmartObject, IRendersBrush
    {
        [DataMember(Name=nameof(Id), Order=0)]
        private Guid _id = Guid.NewGuid();
        
        [DataMember(Name=nameof(FullName), Order=1)]
        private string? _fullName;
        
        [DataMember(Name=nameof(ShortName), Order=2)]
        private string? _shortName;
        
        [DataMember(Name=nameof(FileExtension), Order=3)]
        private string? _fileExtension;
        
        [DataMember(Name=nameof(FileSize), Order=4)]
        private long _fileSize;
        
        [DataMember(Name=nameof(PixelWidth), Order=5)]
        private int _pixelWidth;
        
        [DataMember(Name=nameof(PixelHeight), Order=6)]
        private int _pixelHeight;
        
        [DataMember(Name=nameof(Path), Order=7)]
        private string? _path;
        
        private Brush? _renderBrush;
        private BitmapImage? _bitmap;
        
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

        [NotMapped]
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