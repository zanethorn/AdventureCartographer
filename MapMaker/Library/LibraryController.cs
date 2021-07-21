#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using MapMaker.Annotations;
using MapMaker.File;
using Microsoft.EntityFrameworkCore;
using Image = System.Drawing.Image;

namespace MapMaker.Library
{
    public class LibraryController : SmartObject
    {
        public static readonly string[] FileExtensions = new[] {".png", ".jpg", ".bmp"};
        private LibraryDbContext _context;
        private string _libraryName = "Default";
        private ImageCollection _selectedCollection;
        private ImageCollection _defaultCollection;
        private string _scanPath= string.Empty;
        private bool _scanSubfolders;
        private CollectionModes _collectionMode = CollectionModes.DefaultCollection;
        private SmartCollection<ImageCollection> _imageCollections = new();
        private SmartCollection<ImageFile> _allImages = new();
        private string _imageSearch= string.Empty;

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

        public SmartCollection<ImageCollection> ImageCollections
        {
            get => _imageCollections;
            private set
            {
                if (value == _imageCollections) return;
                _imageCollections = value;
                OnPropertyChanged();
            }
        }

        public SmartCollection<ImageFile> AllImages
        {
            get => _allImages;
            private set
            {
                if (value == _allImages) return;
                _allImages = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredImages));
            }
        }

        public string ImageSearch
        {
            get => _imageSearch;
            set
            {
                if (value == _imageSearch) return;
                _imageSearch = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredImages));
            }
        }

        public IList<ImageFile> FilteredImages =>
            string.IsNullOrWhiteSpace(ImageSearch)
                ? _allImages
                : _allImages.Where(i => i.ShortName.StartsWith(ImageSearch, StringComparison.InvariantCultureIgnoreCase)).ToList();

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

            await _context.ImageCollections.Include(i => i.Images).LoadAsync();
            var imageCollections = new SmartCollection<ImageCollection>(_context.ImageCollections.Local);

            await _context.ImageFiles.LoadAsync();
            AllImages = new SmartCollection<ImageFile>(_context.ImageFiles.Local);

            if (imageCollections.Count == 0)
            {
                await AddCollectionAsync("Default");
            }

            ImageCollections = imageCollections;
            DefaultCollection = ImageCollections[0];
            SelectedCollection = DefaultCollection;
            
            DispatchNotifications();
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
            
            DispatchNotifications();

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
            DispatchNotifications();
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
                var collectionName =
                    Path.GetFileName(Path.GetFullPath(folderPath).TrimEnd(Path.DirectorySeparatorChar));
                var saveCollection = collection ?? await AddCollectionAsync(collectionName);

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

        public async Task<ImageFile> AddImageToCollectionAsync(string imagePath, ImageCollection collection)
        {
            var imgFile = _allImages.SingleOrDefault(i => i.Path == imagePath);
            if (imgFile == null)
            {
                await using var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                using var img = Image.FromStream(fileStream);
                var name = Path.GetFileNameWithoutExtension(imagePath);
                imgFile = new ImageFile()
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
                AllImages.Add(imgFile);
            }
            
            collection.Images.Add(imgFile);
            await _context.SaveChangesAsync();
            DispatchNotifications();

            return imgFile;
        }

        public async Task DeleteImage(ImageFile image)
        {
            _context.ImageFiles.Remove(image);
            AllImages.Remove(image);
            await _context.SaveChangesAsync();
            OnPropertyChanged(nameof(FilteredImages));
            DispatchNotifications();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}