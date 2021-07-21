using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapMaker.Commands;
using MapMaker.File;
using MapMaker.Library;

namespace MapMaker
{
    public partial class MapEditor : UserControl
    {
        private LibraryController _library;
        private MapController _map;

        public MapEditor()
        {
            InitializeComponent();

            _library = (LibraryController) FindResource(nameof(LibraryController));
            _map = (MapController) FindResource(nameof(MapController));
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
            var pos = Controller.SnapToGrid(e.GetPosition(FileView));
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fileNames = (string[]) e.Data.GetData(DataFormats.FileDrop);

                Task.Run(async () =>
                {
                    foreach (var file in fileNames)
                    {
                        var imgFile = await _library.AddImageToCollectionAsync(file, _library.DefaultCollection);
                        var imgObject = new MapImage()
                        {
                            Image = imgFile,
                            Offset = pos,
                            Size = new Size(
                                imgFile.PixelWidth,
                                imgFile.PixelHeight
                            )
                        };
                        var command = new AddImageCommand(imgObject, _map.SelectedLayer);
                        _map.IngestCommand(command);
                    }
                });
            }
            else if (e.Data.GetDataPresent(typeof(ImageFile)))
            {
                var data = e.Data.GetData(typeof(ImageFile));
                if (data is ImageFile imgFile)
                {
                    var imgObject = new MapImage()
                    {
                        Image = imgFile,
                        Offset = pos,
                        Size = new Size(
                            imgFile.PixelWidth,
                            imgFile.PixelHeight
                        )
                    };
                    var command = new AddImageCommand(imgObject, Controller.SelectedLayer);
                    _map.IngestCommand(command);
                }
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Controller.SelectedTool.Move(e.GetPosition(FileView));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Controller.SelectedTool.Down(e.GetPosition(FileView));
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Controller.SelectedTool.Up(e.GetPosition(FileView));
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Controller.SelectedTool.Up(e.GetPosition(FileView));
        }

        private void OnCanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Controller.SelectedObject != null;
        }

        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            var original = (MapObject) Clipboard.GetData(nameof(MapObject));
            var newItem = (MapObject) original.Clone();
            Controller.SelectedLayer.MapObjects.Append(newItem);
        }

        private void OnCanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Clipboard.ContainsData(nameof(MapObject)))
                e.CanExecute = true;
            else
            {
                e.CanExecute = false;
            }
        }
    }
}