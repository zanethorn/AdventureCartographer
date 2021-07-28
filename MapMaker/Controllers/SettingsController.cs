using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MapMaker.Models;

namespace MapMaker.Controllers
{
    public class SettingsController : SmartObject
    {
        private Settings _settings;
        private static FileVersionInfo _versionInfo;

        public static readonly string AppDataPath;
        public static readonly string LibraryRootPath;

        static SettingsController()
        {
            _versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);

            AppDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                _versionInfo.CompanyName,
                _versionInfo.ProductName
            );

            LibraryRootPath = Path.Combine(AppDataPath, "Library");
        }

        public SettingsController()
        {
            Settings = new Settings();
        }

        public FileVersionInfo VersionInfo => _versionInfo;

        public Settings Settings
        {
            get => _settings;
            private set
            {
                if (value == _settings) return;
                if (_settings != null)
                    _settings.PropertyChanged -= OnSettingsChanged;
                _settings = value;
                _settings.PropertyChanged += OnSettingsChanged;
                OnPropertyChanged();
            } 
        }


        public int GridCellWidth
        {
            get => Settings.GridCellWidth;
            set => Settings.GridCellWidth = value;
        }


        public float GridPenWidth
        {
            get => Settings.GridPenWidth;
            set => Settings.GridPenWidth = value;
        }

        public string GridPenColor
        {
            get => Settings.GridPenColor;
            set => Settings.GridPenColor = value;
        }

        public string BackgroundGridColor1
        {
            get => Settings.BackgroundGridColor1;
            set => Settings.BackgroundGridColor1 = value;
        }

        public string BackgroundGridColor2
        {
            get => Settings.BackgroundGridColor2;
            set => Settings.BackgroundGridColor2 = value;
        }


        public bool ShowGrid
        {
            get => Settings.ShowGrid;
            set => Settings.ShowGrid = value;
        }


        public bool SnapToGrid
        {
            get => Settings.ShowGrid;
            set => Settings.ShowGrid = value;
        }


        public string ControlHighlightColor
        {
            get => Settings.ControlHighlightColor;
            set => Settings.ControlHighlightColor = value;
        }

        public Task Init()
        {
            var settingsPath = Path.Combine(AppDataPath, "settings.xml");

            var directory = Path.GetDirectoryName(settingsPath);
            Debug.Assert(directory != null, nameof(directory) + " != null");
            Directory.CreateDirectory(directory);

            var serializer = new XmlSerializer(typeof(Settings));
            if (File.Exists(settingsPath))
                Settings = (Settings) (serializer.Deserialize(File.OpenRead(settingsPath)) ?? new Settings());
            else
                serializer.Serialize(File.OpenWrite(settingsPath), Settings);

            return Task.CompletedTask;
        }

        public Task Save()
        {
            var settingsPath = Path.Combine(AppDataPath, "settings.xml");
            var serializer = new XmlSerializer(typeof(Settings));
            serializer.Serialize(File.OpenWrite(settingsPath), Settings);
            return Task.CompletedTask;
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
    }
}