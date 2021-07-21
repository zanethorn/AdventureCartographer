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
    public class ImageFile : SmartObject
    {
        private Guid _id = Guid.NewGuid();
        private string _fullName;
        private string _shortName;
        private BitmapImage? _bitmap;
        private string _path;
        private int _pixelWidth;
        private int _pixelHeight;
        private string _fileExtension;
        private long _fileSize;

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
        public string Path
        {
            get => _path;
            set
            {
                if (value == _path) return;
                _path = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Bitmap));
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
            get => _fileExtension;
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
                {
                    _bitmap = new BitmapImage(new Uri(Path))
                        {CacheOption = BitmapCacheOption.OnLoad, DecodePixelWidth = 70};
                    _bitmap.Freeze();
                    OnPropertyChanged();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bitmap = null;
            }
            base.Dispose(disposing);
        }
    }
}