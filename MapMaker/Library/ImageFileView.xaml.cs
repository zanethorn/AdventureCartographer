using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapMaker.Library
{
    public partial class ImageFileView : UserControl
    {
        public ImageFileView()
        {
            InitializeComponent();
        }

        public LibraryImage File
        {
            get => (LibraryImage) DataContext;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, File, DragDropEffects.Link);
            }
        }
    }
}