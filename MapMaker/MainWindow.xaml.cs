using System;
using System.Collections.Generic;
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
using System.Xml;
using System.Xml.Serialization;
using MapMaker.File;
using MapMaker.Library;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

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

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            // TODO: Show Loading Screen Here
            _libraryController.LoadLibraryAsync().Wait();
            // TODO: Hide Loading Screen

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
                using var file = System.IO.File.Create(_lastFileSaveName);
                var serializer = new XmlSerializer(typeof(MapFile));
                serializer.Serialize(file,_mapController.MapFile);
            }
        }

        private void OnSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _lastFileSaveName = dialog.FileName;
                OnSave(sender, e);
            }
        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _lastFileSaveName = dialog.FileName;
                using var file = System.IO.File.OpenRead(_lastFileSaveName);
                var serializer = new XmlSerializer(typeof(MapFile));
                _mapController.MapFile=(MapFile)serializer.Deserialize(file);
            }
        }

        
    }
}