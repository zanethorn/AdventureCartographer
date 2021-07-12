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
using MapMaker.File;
using Microsoft.EntityFrameworkCore;

namespace MapMaker.Library
{
    public class LibraryController:INotifyPropertyChanged
    {
        public static readonly string[] FileExtensions = new[] {".png", ".jpg", ".bmp"};
        private LibraryDbContext? _context;
        private bool _isLoaded;
        private string _libraryName;
        private ObservableCollection<ImageCollection> _imageCollections = new ObservableCollection<ImageCollection>();
        private ObservableCollection<ImageFile> _allFiles = new ObservableCollection<ImageFile>();

        public event PropertyChangedEventHandler PropertyChanged;
        
        public bool IsLoaded
        {
            get => _isLoaded;
            private set
            {
                _isLoaded = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmpty => ImageCollections.Count == 0;

        public string LibraryName
        {
            get => _libraryName;
            private set
            {
                _libraryName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ImageCollection> ImageCollections
        {
            get => _imageCollections;
            private set
            {
                _imageCollections = value;
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<ImageFile> AllFiles
        {
            get => _allFiles;
            private set
            {
                _allFiles = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadLibrary(string? libraryName = null)
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
            
            await _context.ImageCollections.LoadAsync();
            ImageCollections = new ObservableCollection<ImageCollection>(_context.ImageCollections.Local);

            await _context.ImageFiles.LoadAsync();
            AllFiles = new ObservableCollection<ImageFile>(_context.ImageFiles.Local);

            IsLoaded = true;
        }

        public void ScanImageFolder(string folderPath, bool recursive = false)
        {
            if (folderPath == null) throw new ArgumentNullException(nameof(folderPath));
            
            foreach (var file in Directory.GetFiles(folderPath))
            {
                var ext = Path.GetExtension(file);
                if (FileExtensions.Contains(ext))
                {
                    AddImageToLibrary(file);
                }
            }
        }

        public void AddImageToLibrary(string imagePath)
        {
            Debug.Assert(_context != null, nameof(_context) + " != null");
            
            var imgFile = new ImageFile()
            {
                Path = imagePath
            };

            
            _context.ImageFiles.Add(imgFile);

            //_context.SaveChanges();
        }

        public void CloseLibrary()
        {
            if (_context != null)
            {
                IsLoaded = false;
                _context.Dispose();
                _context = null;
            }
        }
        
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}