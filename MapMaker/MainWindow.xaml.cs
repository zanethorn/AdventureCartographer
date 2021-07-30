using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using MapMaker.Controllers;
using MapMaker.Models.Library;
using MapMaker.Models.Map;
using MapMaker.Views;
using MapMaker.Views.Dialogs;
using MonitoredUndo;
using Ookii.Dialogs.Wpf;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace MapMaker
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EditorController _editorController;
        private readonly LibraryController _libraryController;
        private readonly MapController _mapController;
        private readonly SettingsController _settingsController;
        private string? _lastExportName;
        private string? _lastFileSaveName;
        private ProgressDialog? _progressDialog;

        public MainWindow()
        {
            InitializeComponent();
            _settingsController = (SettingsController) FindResource(nameof(SettingsController));
            _libraryController = (LibraryController) FindResource(nameof(LibraryController));
            _mapController = (MapController) FindResource(nameof(MapController));
            _mapController.PropertyChanged += OnControllerPropertyChanged;
            _editorController = (EditorController) FindResource(nameof(EditorController));
        }

        private void OnScanDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new ScanDirectoryDialog();
            if (dialog.ShowDialog() == true)
            {
                var oldFileCount = _libraryController.AllImages.Count;
                var oldCollectionCount = _libraryController.ImageCollections.Count;
                _libraryController.ScanImageFolderAsync().Wait();
                var newFileCount = _libraryController.AllImages.Count;
                var newCollectionCount = _libraryController.ImageCollections.Count;

                var addFileCount = newFileCount - oldFileCount;
                var addCollectionCount = newCollectionCount - oldCollectionCount;

                MessageBox.Show(this,
                    $"{addFileCount} Files Added To Library in {Math.Max(1, addCollectionCount)} collections.",
                    "Scan Files");
            }
        }

        private void OnShowPreferences(object sender, RoutedEventArgs e)
        {
            var dialog = new PreferencesDialog();
            dialog.ShowDialog();
        }

        private void OnShowAbout(object sender, RoutedEventArgs e)
        {
            var dialog = new SplashDialog();
            dialog.ShowDialog();
        }

        private void OnViewLibraryDetails(object sender, RoutedEventArgs e)
        {
            var dialog = new LibraryDetailsDialog();
            dialog.ShowDialog();
        }

        private void OnExit(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_libraryController.IsEmpty)
                // TODO: Put this into a localizable file
                if (MessageBox.Show(this,
                    "It looks like there are no image files currently in your image library.  Would you like to scan a directory to add some?",
                    "Add Library Files",
                    MessageBoxButton.YesNo
                ) == MessageBoxResult.Yes)
                    if (CustomCommands.ScanDirectory.CanExecute(null, this))
                        CustomCommands.ScanDirectory.Execute(null, this);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _libraryController.Dispose();
        }

        private void OnNew(object sender, ExecutedRoutedEventArgs e)
        {
            var result = MessageBox.Show(
                this,
                "Do you wish to save the current file before creating a new one?",
                "New File", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.Yes:
                    OnSave(sender, e);
                    break;
            }

            _mapController.NewMap();
        }

        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_lastFileSaveName))
                OnSaveAs(sender, e);
            else
            {
                var progressDialog = new ProgressDialog();
                progressDialog.Show();
                Task.Run(async () =>
                {
                    await _mapController.SaveMap(_lastFileSaveName, _editorController.SelectedMap);
                    Dispatcher.BeginInvoke(() => progressDialog.Dispose());
                });
            }
        }

        private void OnSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                FileName = _editorController.SelectedMap.Name,
                Filter = "map files (*.mapx)|*.mapx|All files (*.*)|*.*",
                OverwritePrompt = true,
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _lastFileSaveName = dialog.FileName;
                OnSave(sender, e);
            }
        }

        private void OnExport(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_lastExportName))
            {
                OnExportAs(sender, e);
            }
            else
            {
                var bmp = new RenderTargetBitmap(
                    _editorController.SelectedMap.PixelWidth,
                    _editorController.SelectedMap.PixelHeight,
                    96,
                    96,
                    PixelFormats.Pbgra32);
                var previousGridState = _settingsController.Settings.ShowGrid;
                _settingsController.Settings.ShowGrid = _editorController.SelectedMap.ExportGrid;
                bmp.Render(Editor.FileView);
                _settingsController.Settings.ShowGrid = previousGridState;

                var encoder = new PngBitmapEncoder();
                BitmapFrame frame = BitmapFrame.Create(bmp);
                encoder.Frames.Add(frame);

                using var stream = File.Create(_lastExportName);
                encoder.Save(stream);
                MessageBox.Show("Done Exporting File");
            }
        }

        private void OnExportAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                FileName = _editorController.SelectedMap.Name,
                Filter = "png files (*.png)|*.png|All files (*.*)|*.*",
                OverwritePrompt = true,
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _lastExportName = dialog.FileName;
                OnExport(sender, e);
            }
        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "map files (*.mapx)|*.mapx|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _lastFileSaveName = dialog.FileName;
                Task.Run(() => _mapController.LoadMap(_lastFileSaveName));
            }
        }

        private void OnUndo(object sender, ExecutedRoutedEventArgs e)
        {
            _editorController.Undo();
        }

        private void OnCanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _editorController.CanUndo;
            e.Handled = true;
        }

        private void OnRedo(object sender, ExecutedRoutedEventArgs e)
        {
            _editorController.Redo();
        }

        private void OnCanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _editorController.CanRedo;
            e.Handled = true;
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is MapObject mapObject)
            {
                Clipboard.Clear();
                var serializer = new XmlSerializer(typeof(MapObject));
                using StringWriter writer = new();
                serializer.Serialize(writer, mapObject);
                Clipboard.SetData(nameof(MapObject), writer.ToString());
                e.Handled = true;
            }
        }

        private void OnCanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
            e.Handled = true;
        }

        private void OnDuplicate(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                {
                    var newImage = (MapImage) mapObject.Clone();
                    _mapController.AddObjectToLayer(
                        _editorController.SelectedMap,
                        _editorController.SelectedLayer,
                        newImage
                    );
                    _editorController.SelectedObject = newImage;
                    break;
                }
                case MapLayer mapLayer:
                {
                    var newLayer = (MapLayer) mapLayer.Clone();
                    _mapController.AddLayer(
                        _editorController.SelectedMap,
                        newLayer
                    );
                    _editorController.SelectedLayer = newLayer;
                    break;
                }
            }
        }

        private void OnCanDuplicate(object sender, CanExecuteRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapImage:
                case MapLayer:
                    e.CanExecute = true;
                    break;
            }

            e.Handled = true;
        }

        private void OnCut(object sender, ExecutedRoutedEventArgs e)
        {
            OnCopy(sender, e);
            OnDelete(sender, e);
        }

        private void OnCanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
            e.Handled = true;
        }

        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            if (Clipboard.ContainsData(nameof(MapObject)))
            {
                var serializer = new XmlSerializer(typeof(MapObject));
                using StringReader reader = new(Clipboard.GetData(nameof(MapObject)).ToString() ?? string.Empty);
                var mapObject = (MapObject) serializer.Deserialize(reader)!;

                _mapController.AddObjectToLayer(
                    _editorController.SelectedMap,
                    _editorController.SelectedLayer,
                    mapObject
                );
                _editorController.SelectedObject = mapObject;

                e.Handled = true;
            }
        }

        private void OnCanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(nameof(MapObject));
            e.Handled = true;
        }

        private void OnDelete(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                {
                    _mapController.DeleteObjectFromLayer(
                        _editorController.SelectedMap,
                        _editorController.SelectedLayer,
                        mapObject
                    );
                    if (mapObject == _editorController.SelectedObject)
                        _editorController.SelectedObject = null;
                    break;
                }
                case LibraryImage libraryImage:
                {
                    if (MessageBox.Show(
                        "Are you sure you wish to delete this image from your library?",
                        "Delete Image",
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        _libraryController.DeleteImage(libraryImage).Wait();
                    break;
                }
                case MapLayer mapLayer:
                {
                    var ix = _editorController.SelectedMap.Layers.IndexOf(_editorController.SelectedLayer);
                    _mapController.DeleteLayer(
                        _editorController.SelectedMap,
                        mapLayer
                    );
                    if (mapLayer == _editorController.SelectedLayer)
                        _editorController.SelectedLayer = _editorController.SelectedMap.Layers[ix];
                    break;
                }
            }
        }

        private void OnCanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject:
                case LibraryImage:
                    e.CanExecute = true;
                    break;
                case MapLayer:
                    e.CanExecute = _editorController.SelectedMap.Layers.Count > 1;
                    break;
            }

            e.Handled = true;
        }


        private void OnMoveUp(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                {
                    _mapController.MoveObjectUp(
                        _editorController.SelectedMap,
                        _editorController.SelectedLayer,
                        mapObject
                    );
                    break;
                }
                case MapLayer mapLayer:
                {
                    _mapController.MoveLayerUp(
                        _editorController.SelectedMap,
                        mapLayer
                    );
                    break;
                }
            }
        }

        private void OnCanMoveUp(object sender, CanExecuteRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                    e.CanExecute = _editorController.SelectedLayer.MapObjects.IndexOf(mapObject) <
                                   _editorController.SelectedLayer.MapObjects.Count - 1;
                    break;
                case MapLayer layer:
                    e.CanExecute = _editorController.SelectedMap.Layers.IndexOf(layer) <
                                   _editorController.SelectedMap.Layers.Count - 1;
                    break;
            }

            e.Handled = true;
        }

        private void OnMoveDown(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                {
                    _mapController.MoveObjectDown(
                        _editorController.SelectedMap,
                        _editorController.SelectedLayer,
                        mapObject
                    );
                    break;
                }
                case MapLayer mapLayer:
                {
                    _mapController.MoveLayerDown(
                        _editorController.SelectedMap,
                        mapLayer
                    );
                    break;
                }
            }
        }

        private void OnCanMoveDown(object sender, CanExecuteRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                    e.CanExecute = _editorController.SelectedLayer.MapObjects.IndexOf(mapObject) > 0;
                    break;
                case MapLayer layer:
                    e.CanExecute = _editorController.SelectedMap.Layers.IndexOf(layer) > 0;
                    break;
            }

            e.Handled = true;
        }

        private void OnMoveTop(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                {
                    _mapController.MoveObjectTop(
                        _editorController.SelectedMap,
                        _editorController.SelectedLayer,
                        mapObject
                    );
                    break;
                }
                case MapLayer mapLayer:
                {
                    _mapController.MoveLayerTop(
                        _editorController.SelectedMap,
                        mapLayer
                    );
                    break;
                }
            }
        }

        private void OnMoveBottom(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                {
                    _mapController.MoveObjectBottom(
                        _editorController.SelectedMap,
                        _editorController.SelectedLayer,
                        mapObject
                    );
                    break;
                }
                case MapLayer mapLayer:
                {
                    _mapController.MoveLayerBottom(
                        _editorController.SelectedMap,
                        mapLayer
                    );
                    break;
                }
            }
        }

        private void OnNewLayer(object sender, ExecutedRoutedEventArgs e)
        {
            var newLayer = new MapLayer
            {
                Name = $"UntitledLayer_{_editorController.SelectedMap.Layers.Count + 1}"
            };
            _mapController.AddLayer(_editorController.SelectedMap, newLayer);
        }
        
        private void OnNewBrush(object sender, ExecutedRoutedEventArgs e)
        {
            var brush = new MapBrush();
            _mapController.AddBrush(_editorController.SelectedMap,brush);
            _editorController.SelectedBrush = brush;
        }

        private void OnControllerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditorController.CanUndo) ||
                e.PropertyName == nameof(EditorController.CanRedo))
                CommandManager.InvalidateRequerySuggested();
        }

        private void OnShowMapSettings(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new MapSettingsDialog();
            dialog.ShowDialog();
        }
    }
}