using System.Windows;

namespace MapMaker.Views.Dialogs
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