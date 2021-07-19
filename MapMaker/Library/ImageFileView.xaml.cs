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

        public ImageFile File
        {
            get => (ImageFile) DataContext;
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