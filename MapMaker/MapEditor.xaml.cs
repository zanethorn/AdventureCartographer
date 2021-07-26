using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapMaker.Commands;
using MapMaker.Library;
using MapMaker.Map;
using MapMaker.Properties;

namespace MapMaker
{
    public partial class MapEditor : UserControl
    {
        private LibraryController _library;
        private MapController _map;
        private Point _lastPosition;
        private Point _downPosition;
        private Point _upPosition;

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
            else if (e.Data.GetDataPresent(typeof(LibraryImage)))
            {
                var data = e.Data.GetData(typeof(LibraryImage));
                if (data is LibraryImage imgFile)
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
            var thisPosition = e.GetPosition(FileView);
            var moveOffset = _lastPosition - thisPosition;
            Controller.CursorPosition = thisPosition;
            Cursor cursor = Cursors.Arrow;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IMapCommand command = null;
                switch (Controller.SelectedTool)
                {
                    case ToolTypes.Pointer:
                    {
                        if (Controller.SelectedObject != null)
                        {
                            cursor = Cursors.ScrollAll;
                            command = new DragResizeObjectCommand(Controller.SelectedObject,
                                Controller.SelectedObject.Offset - moveOffset ,
                                Controller.SelectedObject.Size);
                        }
                        else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                        {
                            cursor = Cursors.ScrollAll;
                            command = new PanFileViewCommand(Controller.Offset - moveOffset);
                        }
                    }
                        break;
                    case ToolTypes.Pan:
                    {
                        cursor = Cursors.ScrollAll;
                        command = new PanFileViewCommand(Controller.Offset - moveOffset);
                    }
                        break;
                    case ToolTypes.Shape:
                    {
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(MapController.SelectedTool));
                }

                if (command != null)
                {
                    Controller.IngestCommand(command);
                }
            }
            else
            {
                switch (Controller.SelectedTool)
                {
                    case ToolTypes.Pointer:
                    {
                        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                        {
                            cursor = Cursors.Hand;
                        }
                    }
                        break;
                    case ToolTypes.Pan:
                    {
                        cursor = Cursors.Hand;
                    }
                        break;
                    case ToolTypes.Shape:
                    {
                        cursor = Cursors.Pen;
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(MapController.SelectedTool));
                }
            }

            Cursor = cursor;
            _lastPosition = thisPosition;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _downPosition = e.GetPosition(FileView);
            IMapCommand command = null;
            switch (Controller.SelectedTool)
            {
                case ToolTypes.Pointer:
                {
                }
                    break;
                case ToolTypes.Pan:
                {
                }
                    break;
                case ToolTypes.Shape:
                {
                    var shape = new MapShape()
                    {
                        Offset = _downPosition,
                        Size = new Size(3 * Settings.Default.GridCellWidth, 3 * Settings.Default.GridCellWidth)
                    };
                    command = new AddObjectCommand(shape, Controller.SelectedLayer);
                    Controller.SelectObject(shape,true);
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(MapController.SelectedTool));
            }

            if (command != null)
            {
                Controller.IngestCommand(command);
            }

            _lastPosition = _downPosition;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _upPosition = e.GetPosition(FileView);
            var moveOffset = _lastPosition - _upPosition;
            Controller.CursorPosition = _upPosition;
            IMapCommand command = null;
            switch (Controller.SelectedTool)
            {
                case ToolTypes.Pointer:
                {
                    command = new DragResizeObjectCommand(Controller.SelectedObject,
                        Controller.SnapToGrid(Controller.SelectedObject.Offset - moveOffset) ,
                        Controller.SelectedObject.Size);
                }
                    break;
                case ToolTypes.Pan:
                {
                }
                    break;
                case ToolTypes.Shape:
                {
                    
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(MapController.SelectedTool));
            }

            if (command != null)
            {
                Controller.IngestCommand(command);
            }

            _lastPosition = _upPosition;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            //Controller.SelectedTool.Up(e.GetPosition(FileView));
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