using System;
using System.Collections.Generic;
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
using MapMaker.File;
using MapMaker.Library;
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
        private ProgressDialog _progressDialog;
        
        public MainWindow()
        {
            InitializeComponent();
            _libraryController = (LibraryController) FindResource(nameof(LibraryController));
            _mapController = (MapController) FindResource(nameof(MapController));
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
            _libraryController.CloseLibrary();
        }
        
        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_lastFileSaveName))
            {
                OnSaveAs(sender, e);
            }
            else
            {
                MapLoader.Save(_lastFileSaveName, _mapController.MapFile);
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
                _mapController.MapFile = MapLoader.Load(_lastFileSaveName);
            }
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
                _mapController.AddObject(mapObject);
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
            if (e.Parameter is MapObject mapObject)
            {
                _mapController.SelectedLayer.MapObjects.Remove(mapObject);
                e.Handled = true;
            }
        }

        private void OnCanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is MapObject mapObject)
            {
                e.CanExecute = true;
                
            }
            e.Handled = true;
        }
    }
}