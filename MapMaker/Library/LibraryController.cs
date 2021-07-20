#nullable enable
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using MapMaker.Annotations;
using MapMaker.File;
using Microsoft.EntityFrameworkCore;
using Image = System.Drawing.Image;

namespace MapMaker.Library
{
    public class LibraryController : INotifyPropertyChanged
    {
        public static readonly string[] FileExtensions = new[] {".png", ".jpg", ".bmp"};
        private LibraryDbContext? _context;
        private string _libraryName;
        private ImageCollection _selectedCollection;
        private ImageCollection _defaultCollection;
        private string _scanPath;
        private bool _scanSubfolders;
        private CollectionModes _collectionMode = CollectionModes.DefaultCollection;
        private ObservableCollection<ImageCollection> _imageCollections = new ObservableCollection<ImageCollection>();
        private ObservableCollection<ImageFile> _allImages = new ObservableCollection<ImageFile>();
        

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsEmpty => AllImages.Count == 0;

        public string LibraryName
        {
            get => _libraryName;
            private set
            {
                if (value == _libraryName) return;
                _libraryName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ImageCollection> ImageCollections
        {
            get => _imageCollections;
            private set
            {
                if (value == _imageCollections) return;
                _imageCollections = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ImageFile> AllImages
        {
            get => _allImages;
            private set
            {
                if (value == _allImages) return;
                _allImages = value;
                OnPropertyChanged();
            }
        }

        public ImageCollection SelectedCollection
        {
            get => _selectedCollection;
            set
            {
                if (value == _selectedCollection) return;
                _selectedCollection = value;
                OnPropertyChanged();
            }
        }

        public ImageCollection DefaultCollection
        {
            get => _defaultCollection;
            private set
            {
                if (Equals(value, _defaultCollection)) return;
                _defaultCollection = value;
                OnPropertyChanged();
            }
        }


        public string ScanPath
        {
            get => _scanPath;
            set
            {
                if (value == _scanPath) return;
                _scanPath = value;
                OnPropertyChanged();
            }
        }

        public bool ScanSubfolders
        {
            get => _scanSubfolders;
            set
            {
                if (value == _scanSubfolders) return;
                _scanSubfolders = value;
                OnPropertyChanged();
            }
        }

        public CollectionModes CollectionMode
        {
            get => _collectionMode;
            set
            {
                if (value == _collectionMode) return;
                _collectionMode = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadLibraryAsync(string? libraryName = null)
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
            Debug.Assert(versionInfo.CompanyName != null, "versionInfo.CompanyName != null");
            Debug.Assert(versionInfo.ProductName != null, "versionInfo.ProductName != null");

            if (string.IsNullOrWhiteSpace(libraryName))
            {
                libraryName = Properties.Settings.Default.DefaultLibraryName;
            }

            LibraryName = libraryName;

            libraryName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                versionInfo.CompanyName,
                versionInfo.ProductName,
                "Library",
                $"{libraryName}.db"
            );

            var directory = Path.GetDirectoryName(libraryName);
            Debug.Assert(directory != null, nameof(directory) + " != null");
            Directory.CreateDirectory(directory);

            _context = new LibraryDbContext(libraryName);
            await _context.Database.EnsureCreatedAsync();

            await _context.ImageCollections.Include(i=>i.Images).LoadAsync();
            var imageCollections = new ObservableCollection<ImageCollection>(_context.ImageCollections.Local);

            await _context.ImageFiles.LoadAsync();
            AllImages = new ObservableCollection<ImageFile>(_context.ImageFiles.Local);

            if (imageCollections.Count == 0)
            {
                await AddCollectionAsync("Default");
            }

            ImageCollections = imageCollections;
            DefaultCollection = ImageCollections[0];
            SelectedCollection = DefaultCollection;
        }

        public async Task<ImageCollection> AddCollectionAsync(string? name)
        {

            var newCollection = new ImageCollection
            {
                Name = name
            };


            await _context.ImageCollections.AddAsync(newCollection);
            await _context.SaveChangesAsync();
            ImageCollections.Add(newCollection);

            return newCollection;
        }

        public async Task ScanImageFolderAsync()
        {
            var collection = CollectionMode switch
            {
                CollectionModes.DefaultCollection => DefaultCollection,
                CollectionModes.NewCollection => await AddCollectionAsync($"New Collection {ImageCollections.Count}"),
                CollectionModes.PerFolder => null,
                _ => throw new ArgumentOutOfRangeException()
            };

            await ScanImageFolderAsync(ScanPath, ScanSubfolders, collection);
        }

        private async Task ScanImageFolderAsync(string folderPath, bool recursive, ImageCollection? collection)
        {
            if (folderPath == null) throw new ArgumentNullException(nameof(folderPath));

            var files = Directory
                .GetFiles(folderPath)
                .Where(i => FileExtensions.Contains(Path.GetExtension(i)))
                .ToList();

            if (files.Count > 0)
            {
                var saveCollection = collection ?? await AddCollectionAsync(folderPath[(folderPath.LastIndexOf(Path.PathSeparator)+1)..]);

                foreach (var file in files)
                {
                    await AddImageToCollectionAsync(file, saveCollection);
                }
            }
            
            if (recursive)
            {
                foreach (var directory in Directory.GetDirectories(folderPath))
                {
                    await ScanImageFolderAsync(directory, recursive, collection);
                }
            }
        }

        public async Task AddImageToCollectionAsync(string imagePath, ImageCollection collection)
        {
            await using var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            using var img = Image.FromStream(fileStream);
            
            var name = Path.GetFileNameWithoutExtension(imagePath);
            
            var imgFile = new ImageFile()
            {
                Path = imagePath,
                FullName = name,
                ShortName = name,
                PixelHeight = img.Height,
                PixelWidth = img.Width,
                FileExtension = Path.GetExtension(imagePath).Substring(1).ToUpper(),
                FileSize = fileStream.Length
            };
            
            

            await _context.ImageFiles.AddAsync(imgFile);
            collection.Images.Add(imgFile);
            await _context.SaveChangesAsync();
            AllImages.Add(imgFile);
        }

        public void CloseLibrary()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}