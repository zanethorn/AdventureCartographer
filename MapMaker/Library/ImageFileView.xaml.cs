using System.Windows.Controls;

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
            set => DataContext = value;
        }
    }
}