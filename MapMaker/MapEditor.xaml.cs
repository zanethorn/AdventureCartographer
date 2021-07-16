using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapMaker.File;
using MapMaker.Library;

namespace MapMaker
{
    public partial class MapEditor : UserControl
    {
        public MapEditor()
        {
            InitializeComponent();

            
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Controller.Offset = new Point(
                (ActualWidth / 2.0) - (Controller.MapFile.PixelWidth * Controller.Scale / 2.0),
                (ActualHeight / 2.0) - (Controller.MapFile.PixelHeight * Controller.Scale / 2.0)
            );
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
                var pos = e.GetPosition(fileView);
                var imgObject = new MapImage()
                {
                    Image = imgFile,
                    Offset=pos,
                    PixelWidth = imgFile.PixelWidth,
                    PixelHeight = imgFile.PixelHeight
                };
                Controller.SelectedLayer.MapObjects.Add(imgObject);
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Controller.SelectedTool.Move(e.GetPosition(fileView));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Controller.SelectedTool.Down(e.GetPosition(fileView));
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Controller.SelectedTool.Up(e.GetPosition(fileView));
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Controller.SelectedTool.Up(e.GetPosition(fileView));
        }
    }
}