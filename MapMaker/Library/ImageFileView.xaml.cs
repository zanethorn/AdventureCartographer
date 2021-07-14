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

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}