using System.Windows;
using System.Windows.Forms;

namespace MapMaker.Library
{
    public partial class ScanDirectoryDialog : Window
    {
        public ScanDirectoryDialog()
        {
            InitializeComponent();
        }

        public LibraryController Controller => (LibraryController) DataContext;

        private void ScanPath_Click(object sender, RoutedEventArgs e)
        {
	        var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Controller.ScanPath = dialog.SelectedPath;
            }
        }
        
        private void OnDefaultClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
		
	}
}