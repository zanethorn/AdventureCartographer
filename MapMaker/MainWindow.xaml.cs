using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Markup;
using MapMaker.Commands;
using MapMaker.File;
using MapMaker.Library;
using MapMaker.Properties;
using Ookii.Dialogs.Wpf;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using XamlReader = System.Windows.Markup.XamlReader;
using XamlWriter = System.Windows.Markup.XamlWriter;

namespace MapMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LibraryController _libraryController;
        private readonly MapController _mapController;
        private string? _lastFileSaveName = null;
        private string? _lastExportName = null;
        private ProgressDialog _progressDialog;
        
        public MainWindow()
        {
            InitializeComponent();
            _libraryController = (LibraryController) FindResource(nameof(LibraryController));
            _mapController = (MapController) FindResource(nameof(MapController));
            _mapController.PropertyChanged += OnControllerPropertyChanged;
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
                
                MessageBox.Show(this, $"{addFileCount} Files Added To Library in {Math.Max(1,addCollectionCount)} collections.", "Scan Files");
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
            // TODO: Show Loading Screen Here
            //_progressDialog = new ProgressDialog()
            //{
            //    Text = "Loading Image Library"
            //};
            
            Task.Run(async () =>
            {
                await _libraryController.LoadLibraryAsync();
                Dispatcher.BeginInvoke(new Action(OnLibraryLoaded));
            });
            //_progressDialog.Show(this);
        }

        private void OnLibraryLoaded()
        {
       
            if (_libraryController.IsEmpty)
            {
                // TODO: Put this into a localizable file
                if (MessageBox.Show(this,
                    "It looks like there are no image files currently in your image library.  Would you like to scan a directory to add some?",
                    "Add Library Files",
                    MessageBoxButton.YesNo
                ) == MessageBoxResult.Yes)
                {
                    if (CustomCommands.ScanDirectory.CanExecute(null,this))
                        CustomCommands.ScanDirectory.Execute(null, this);
                }
            }
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
            {
                OnSaveAs(sender, e);
            }
            else
            {
                Task.Run(() => _mapController.SaveMap(_lastFileSaveName));
            }
        }

        private void OnSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
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
                    _mapController.MapFile.PixelWidth, 
                    _mapController.MapFile.PixelHeight, 
                    96, 
                    96, 
                    PixelFormats.Pbgra32);
                var previousGridState = Settings.Default.ShowGrid;
                Settings.Default.ShowGrid = _mapController.MapFile.ExportGrid;
                bmp.Render(Editor.FileView);
                Settings.Default.ShowGrid = previousGridState;
                
                var encoder = new PngBitmapEncoder();
                BitmapFrame frame = BitmapFrame.Create(bmp);
                encoder.Frames.Add(frame);

                using var stream = System.IO.File.Create(_lastExportName);
                encoder.Save(stream);
                MessageBox.Show("Done Exporting File");
            }
        }

        private void OnExportAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
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
            var dialog = new OpenFileDialog()
            {
                Filter = "map files (*.mapx)|*.mapx|All files (*.*)|*.*",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _lastFileSaveName = dialog.FileName;
                Task.Run(()=>_mapController.LoadMap(_lastFileSaveName));
            }
        }
        
        private void OnUndo(object sender, ExecutedRoutedEventArgs e)
        {
            _mapController.Undo();
        }

        private void OnCanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _mapController.CanUndo;
            e.Handled = true;
        }
        
        private void OnRedo(object sender, ExecutedRoutedEventArgs e)
        {
            _mapController.Redo();
        }

        private void OnCanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _mapController.CanRedo;
            e.Handled = true;
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is MapObject mapObject)
            {
                Clipboard.Clear();
                var serializer = new XmlSerializer(typeof(MapObject));
                using StringWriter writer = new();
                serializer.Serialize(writer,mapObject);
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
                case MapImage mapImage:
                {
                    var newImage = (MapImage)mapImage.Clone();
                    var command = new AddImageCommand(newImage, _mapController.SelectedLayer);
                    _mapController.IngestCommand(command);
                    break;
                }
                case MapLayer layer:
                {
                    var newLayer = (MapLayer)layer.Clone();
                    var command = new AddLayerCommand(newLayer, _mapController.MapFile.Layers.IndexOf(layer));
                    _mapController.IngestCommand(command);
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
                using StringReader reader = new(Clipboard.GetData(nameof(MapObject)).ToString());
                var mapObject = (MapObject) serializer.Deserialize(reader);

                var command = new AddImageCommand(mapObject as MapImage, _mapController.SelectedLayer);
                _mapController.IngestCommand(command);
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
                case MapImage mapImage:
                {
                    var command = new DeleteImageCommand(mapImage, _mapController.SelectedLayer);
                    _mapController.IngestCommand(command);
                    break;
                }
                case ImageFile libraryImage:
                {
                    if (MessageBox.Show(
                        "Are you sure you wish to delete this image from your library?",
                        "Delete Image",
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        _libraryController.DeleteImage(libraryImage).Wait();
                        //Task.Run(async () => await _libraryController.DeleteImage(libraryImage));
                    }
                    break;
                }
                case MapLayer layer:
                {
                    var command = new DeleteLayerCommand(layer);
                    _mapController.IngestCommand(command);
                    break;
                }
            }
        }

        private void OnCanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject:
                case ImageFile:
                    e.CanExecute = true;
                    break;
                case MapLayer:
                    e.CanExecute = _mapController.MapFile.Layers.Count > 1;
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
                    var command = new ReorderObjectCommand(mapObject, _mapController.SelectedLayer, _mapController.SelectedLayer.MapObjects.IndexOf(mapObject) +1);
                    _mapController.IngestCommand(command);
                    break;
                }
                case MapLayer layer:
                {
                    var command = new ReorderLayerCommand(layer,_mapController.MapFile.Layers.IndexOf(layer)+1);
                    _mapController.IngestCommand(command);
                    break;
                }
            }
        }

        private void OnCanMoveUp(object sender, CanExecuteRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                    e.CanExecute = _mapController.SelectedLayer.MapObjects.IndexOf(mapObject) < _mapController.SelectedLayer.MapObjects.Count -1;
                    break;
                case MapLayer layer:
                    e.CanExecute = _mapController.MapFile.Layers.IndexOf(layer) < _mapController.MapFile.Layers.Count-1;
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
                    var command = new ReorderObjectCommand(mapObject, _mapController.SelectedLayer, _mapController.SelectedLayer.MapObjects.IndexOf(mapObject) -1);
                    _mapController.IngestCommand(command);
                    break;
                }
                case MapLayer layer:
                {
                    var command = new ReorderLayerCommand(layer,_mapController.MapFile.Layers.IndexOf(layer)-1);
                    _mapController.IngestCommand(command);
                    break;
                }
            }
        }

        private void OnCanMoveDown(object sender, CanExecuteRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case MapObject mapObject:
                    e.CanExecute = _mapController.SelectedLayer.MapObjects.IndexOf(mapObject) >0;
                    break;
                case MapLayer layer:
                    e.CanExecute = _mapController.MapFile.Layers.IndexOf(layer) > 0;
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
                    var command = new ReorderObjectCommand(mapObject, _mapController.SelectedLayer, _mapController.SelectedLayer.MapObjects.Count -1);
                    _mapController.IngestCommand(command);
                    break;
                }
                case MapLayer layer:
                {
                    var command = new ReorderLayerCommand(layer,_mapController.MapFile.Layers.Count-1);
                    _mapController.IngestCommand(command);
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
                    var command = new ReorderObjectCommand(mapObject, _mapController.SelectedLayer, 0);
                    _mapController.IngestCommand(command);
                    break;
                }
                case MapLayer layer:
                {
                    var command = new ReorderLayerCommand(layer,0);
                    _mapController.IngestCommand(command);
                    break;
                }
            }
        }
        
        private void OnNewLayer(object sender, ExecutedRoutedEventArgs e)
        {
            var newLayer = new MapLayer()
            {
                Name = $"UntitledLayer_{_mapController.MapFile.Layers.Count+1}"
            };
            var command = new AddLayerCommand(newLayer);
            _mapController.IngestCommand(command);
        }
        
        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MapController.CanUndo) ||
                e.PropertyName == nameof(MapController.CanRedo))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}