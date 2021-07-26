using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapMaker.Models.Library;

namespace MapMaker.Views
{
    public partial class ImageFileView : UserControl
    {
        public ImageFileView()
        {
            InitializeComponent();
        }

        public LibraryImage File => (LibraryImage) DataContext;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragDrop.DoDragDrop(this, File, DragDropEffects.Link);
        }
    }
}