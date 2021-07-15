using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapMaker.File;
using MapMaker.Library;

namespace MapMaker
{
    public partial class MapEditor : Page
    {
        public MapEditor()
        {
            InitializeComponent();
        }

        public MapController Controller
        {
            get => (MapController) DataContext;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(ImageFile));
            if (data is ImageFile imgFile)
            {
                var pos = e.GetPosition(FileView);
                var imgObject = new MapImage()
                {
                    Image = imgFile,
                    OffsetX = (int)pos.X,
                    OffsetY = (int)pos.Y,
                    PixelWidth = imgFile.PixelWidth,
                    PixelHeight = imgFile.PixelHeight
                };
                Controller.SelectedLayer.MapObjects.Add(imgObject);
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            
        }
        
    }
}