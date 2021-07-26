using System.Windows;

namespace MapMaker.Views
{
    public partial class LibraryDetailsDialog : Window
    {
        public LibraryDetailsDialog()
        {
            InitializeComponent();
        }

        private void OnDefaultClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}