using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using MapMaker.Annotations;
using MapMaker.Commands;
using MapMaker.Properties;

namespace MapMaker.File
{
    public class MapController : INotifyPropertyChanged
    {
        private const string KEY_FILE_NAME = "map.xml";
        private const string IMAGE_DIRECTORY = "Resources/Images/";

        private MapFile _mapFile;
        private double _scale = 0.5;
        private Point _offset;
        private MapLayer _selectedLayer;
        private MapObject? _selectedObject;
        private bool _isLatched;

        private readonly Stack<IMapCommand> _undoStack = new();
        private readonly Stack<IMapCommand> _redoStack = new();
        private IMapCommand? _currentCommand;

        private Tool _selectedTool;
        private readonly IList<Tool> _tools;
        public event PropertyChangedEventHandler PropertyChanged;

        public MapController()
        {
            SelectedTool = new Pointer(this);
            _tools = new List<Tool>
            {
                SelectedTool,
                new Pan(this)
            };

            NewMap();
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public IEnumerable<Tool> Tools => _tools;

        public Tool SelectedTool
        {
            get => _selectedTool;
            set
            {
                if (value == _selectedTool) return;
                _selectedTool = value;
                OnPropertyChanged();
            }
        }


        public MapObject? SelectedObject
        {
            get => _selectedObject;
            private set
            {
                if (Equals(value, _selectedObject)) return;
                _selectedObject = value;
                OnPropertyChanged();
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                if (value.Equals(_scale)) return;
                _scale = value;
                OnPropertyChanged();
            }
        }

        public Point Offset
        {
            get => _offset;
            set
            {
                if (value == _offset) return;
                _offset = value;
                OnPropertyChanged();
            }
        }


        public MapFile MapFile
        {
            get => _mapFile;
            private set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (Equals(value, _mapFile)) return;
                _mapFile = value;
                OnPropertyChanged();
                SelectedLayer = value.Layers[0];
            }
        }

        public MapLayer SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (Equals(value, _selectedLayer)) return;
                _selectedLayer = value;
                OnPropertyChanged();
            }
        }

        public void IngestCommand(IMapCommand command)
        {
            if (_currentCommand != null)
            {
                command = _currentCommand.Update(command);
            }


            _currentCommand = command;
            _currentCommand.Do(this);

            var top = _undoStack.Count == 0 ? null : _undoStack.Peek();
            if (!Equals(_currentCommand, top))
            {
                _undoStack.Push(_currentCommand);
            }

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public void Undo()
        {
            if (CanUndo)
            {
                _currentCommand = _undoStack.Pop();
                _currentCommand.Undo(this);
                _redoStack.Push(_currentCommand);

                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                _currentCommand = _redoStack.Pop();
                _currentCommand.Do(this);
                _undoStack.Push(_currentCommand);

                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));
            }
        }


        public void NewMap()
        {
            var map = new MapFile();
            map.Layers.Add(new MapLayer() {Name = "UntitledLayer_1"});
            MapFile = map;
            
            _undoStack.Clear();
            _redoStack.Clear();
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public async Task LoadMap(string filename, CancellationToken cancellationToken = default)
        {
            using var zip = ZipFile.Open(filename, ZipArchiveMode.Read);

            // write the basic map.xml key file
            var keyEntry = zip.GetEntry(KEY_FILE_NAME) ?? zip.CreateEntry(KEY_FILE_NAME);
            await using Stream keyStream = keyEntry.Open();
            var serializer = new XmlSerializer(typeof(MapFile));
            var mapFile = (MapFile) serializer.Deserialize(keyStream);
            keyStream.Close();

            foreach (var image in mapFile.ImageFiles)
            {
                var entryName = $"{IMAGE_DIRECTORY}{image.Id}.{image.FileExtension}";
                var imgEntry = zip.GetEntry(entryName);
                var outStream = imgEntry.Open();
                var memStream = new MemoryStream();
                await outStream.CopyToAsync(memStream, cancellationToken);
                memStream.Seek(0, SeekOrigin.Begin);
                outStream.Close();

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memStream;
                bitmap.EndInit();
                if (bitmap.CanFreeze)
                    bitmap.Freeze();
                image.Bitmap = bitmap;
            }

            foreach (var layer in mapFile.Layers)
            {
                foreach (var obj in layer.MapObjects)
                {
                    if (obj is MapImage mapImage)
                    {
                        mapImage.Image = mapFile.ImageFiles.SingleOrDefault(i => i.Id == mapImage.Image.Id);
                    }
                }
            }

            MapFile = mapFile;
            
            _undoStack.Clear();
            _redoStack.Clear();
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public async Task SaveMap(string filename, CancellationToken cancellationToken = default)
        {
            using var zip = ZipFile.Open(filename, ZipArchiveMode.Update);

            // write the basic map.xml key file
            var keyEntry = zip.GetEntry(KEY_FILE_NAME) ?? zip.CreateEntry(KEY_FILE_NAME);
            await using Stream keyStream = keyEntry.Open();
            var serializer = new XmlSerializer(typeof(MapFile));
            serializer.Serialize(keyStream, MapFile);
            await keyStream.FlushAsync(cancellationToken);

            // Store off all images
            foreach (var image in MapFile.ImageFiles)
            {
                var entryName = $"{IMAGE_DIRECTORY}{image.Id}.{image.FileExtension}";
                if (zip.GetEntry(entryName) == null)
                {
                    if (image.Bitmap.StreamSource == null)
                    {
                        zip.CreateEntryFromFile(image.Path, entryName);
                    }
                    else
                    {
                        var imgEntry = zip.CreateEntry(entryName);
                        await using var outStream = imgEntry.Open();
                        await image.Bitmap.StreamSource.CopyToAsync(outStream, cancellationToken);
                        await outStream.FlushAsync(cancellationToken);
                    }
                }
            }
        }

        public void SelectObject(MapObject? mapObject, bool latch = false)
        {
            if (mapObject == null)
            {
                if (latch)
                {
                    SelectedObject = null;
                    _isLatched = false;
                }
                else if (!_isLatched)
                {
                    SelectedObject = null;
                }
            }
            else
            {
                SelectedObject = mapObject;
                if (latch)
                {
                    _isLatched = true;
                }
            }
        }

        public Size SnapToGrid(Size input)
        {
            return Settings.Default.ShowGrid && Settings.Default.SnapToGrid
                ? GetClosestGridPoint(input)
                : input;
        }
        
        public Point SnapToGrid(Point input)
        {
            return Settings.Default.ShowGrid && Settings.Default.SnapToGrid
                ? GetClosestGridPoint(input)
                : input;
        }
        
        public Size GetClosestGridPoint(Size input)
        {
            var w = Settings.Default.GridCellWidth;
            var modX = input.Width % w;
            var modY = input.Height % w;
            var baseX = input.Width - modX;
            var baseY = input.Height - modY;
            return new Size(
                modX / w > 0.5 ? baseX + w : baseX,
                modY / w > 0.5 ? baseY + w : baseY);
        }

        public Point GetClosestGridPoint(Point input)
        {
            var w = Settings.Default.GridCellWidth;
            var modX = input.X % w;
            var modY = input.Y % w;
            var baseX = input.X - modX;
            var baseY = input.Y - modY;
            return new Point(
                modX / w > 0.5 ? baseX + w : baseX,
                modY / w > 0.5 ? baseY + w : baseY);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}