using System.Windows;
using System.Windows.Controls;
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
                var pos = e.GetPosition(this);
                var imgObject = new MapImage()
                {
                    Image = imgFile,
                    OffsetX = (int)pos.X,
                    OffsetY = (int)pos.Y
                };
                Controller.SelectedLayer.MapObjects.Add(imgObject);
            }
        }
    }
}