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
        
        
        public MainWindow()
        {
            InitializeComponent();
            _libraryController = (LibraryController) FindResource(nameof(LibraryController));
        }

        private void OnScanDirectory(object sender, RoutedEventArgs e)
        {
            var dialog = new ScanDirectoryDialog();
            if (dialog.ShowDialog() == true)
            {
                
            }

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
	}
}