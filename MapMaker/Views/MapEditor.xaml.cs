using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapMaker.Controllers;
using MapMaker.Models.Library;
using MapMaker.Models.Map;
using MonitoredUndo;

namespace MapMaker.Views
{
    public partial class MapEditor : UserControl
    {
        private readonly EditorController _editorController;
        private readonly LibraryController _libraryController;
        private readonly MapController _mapController;
        private readonly SettingsController _settingsController;
        private Point _downPosition;
        private Point _lastPosition;
        private Point _upPosition;

        public MapEditor()
        {
            InitializeComponent();

            _libraryController = (LibraryController) FindResource(nameof(LibraryController));
            _mapController = (MapController) FindResource(nameof(MapController));
            _editorController = (EditorController) FindResource(nameof(EditorController));
            _settingsController = (SettingsController) FindResource(nameof(SettingsController));
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (_editorController != null)
            {
                _editorController.Offset = new Point(
                    ActualWidth / 2.0 - _editorController.SelectedMap.PixelWidth * _editorController.Scale / 2.0,
                    ActualHeight / 2.0 - _editorController.SelectedMap.PixelHeight * _editorController.Scale / 2.0
                );
            }
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            var pos = _editorController.SnapToGrid(e.GetPosition(FileView));
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fileNames = (string[]) e.Data.GetData(DataFormats.FileDrop);

                Task.Run(async () =>
                {
                    foreach (var file in fileNames)
                    {
                        var imgFile =
                            await _libraryController.AddImageToCollectionAsync(file,
                                _libraryController.DefaultCollection);
                        var imgObject = new MapImage
                        {
                            Image = imgFile,
                            Offset = pos,
                            Size = new Size(
                                imgFile.PixelWidth,
                                imgFile.PixelHeight
                            )
                        };
                        _mapController.AddObjectToLayer(
                            _editorController.SelectedMap,
                            _editorController.SelectedLayer,
                            imgObject
                        );
                        _editorController.SelectedObject = imgObject;
                    }
                });
            }
            else if (e.Data.GetDataPresent(typeof(LibraryImage)))
            {
                var data = e.Data.GetData(typeof(LibraryImage));
                if (data is LibraryImage imgFile)
                {
                    var imgObject = new MapImage
                    {
                        Image = imgFile,
                        Offset = pos,
                        Size = new Size(
                            imgFile.PixelWidth,
                            imgFile.PixelHeight
                        )
                    };
                    _mapController.AddObjectToLayer(
                        _editorController.SelectedMap,
                        _editorController.SelectedLayer,
                        imgObject
                        );
                    _editorController.SelectedObject = imgObject;
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
            _editorController.CursorPosition = thisPosition;
            Cursor cursor = Cursors.Arrow;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (_editorController.SelectedTool)
                {
                    case ToolTypes.Pointer:
                    {
                        if (_editorController.SelectedObject != null)
                        {
                            using (new UndoBatch(_editorController.SelectedMap,
                                $"Move {_editorController.SelectedObject}", true))
                            {
                                _editorController.SelectedObject.Offset -= moveOffset;
                            }
                        }
                        else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                        {
                            cursor = Cursors.ScrollAll;
                            _editorController.Offset -= moveOffset;
                        }
                    }
                        break;
                    case ToolTypes.Pan:
                    {
                        cursor = Cursors.ScrollAll;
                        _editorController.Offset -= moveOffset;
                    }
                        break;
                    case ToolTypes.Shape:
                    {
                    }
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }
            else
            {
                switch (_editorController.SelectedTool)
                {
                    case ToolTypes.Pointer:
                    {
                        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) cursor = Cursors.Hand;
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
                        throw new InvalidEnumArgumentException();
                }
            }

            Cursor = cursor;
            _lastPosition = thisPosition;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _downPosition = e.GetPosition(FileView);
            switch (_editorController.SelectedTool)
            {
                case ToolTypes.Pointer:
                case ToolTypes.Pan:
                {
                }
                    break;
                case ToolTypes.Shape:
                {
                    var shape = new MapShape
                    {
                        Offset = _downPosition,
                        Size = new Size(3 * _settingsController.Settings.GridCellWidth,
                            3 * _settingsController.Settings.GridCellWidth)
                    };
                    _mapController.AddObjectToLayer(
                        _editorController.SelectedMap,
                        _editorController.SelectedLayer,
                        shape
                        );
                    _editorController.SelectedObject = shape;
                }
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
            _lastPosition = _downPosition;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _upPosition = e.GetPosition(FileView);
            var moveOffset = _lastPosition - _upPosition;
            _editorController.CursorPosition = _upPosition;

            switch (_editorController.SelectedTool)
            {
                case ToolTypes.Pointer:
                {
                    if (_editorController.SelectedObject != null)
                    {
                        using (new UndoBatch(_editorController.SelectedMap,
                            $"Move {_editorController.SelectedObject}", true))
                        {
                            _editorController.SelectedObject.Offset =
                                _editorController.SnapToGrid(_editorController.SelectedObject.Offset - moveOffset);
                        }
                    }
                }
                    break;
                case ToolTypes.Pan:
                case ToolTypes.Shape:
                {
                }
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
            _lastPosition = _upPosition;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            //Controller.SelectedTool.Up(e.GetPosition(FileView));
        }

        private void OnCanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _editorController.SelectedObject != null;
        }

        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            var original = (MapObject) Clipboard.GetData(nameof(MapObject));
            var newItem = (MapObject) original.Clone();
            _mapController.AddObjectToLayer(
                _editorController.SelectedMap,
                _editorController.SelectedLayer,
                newItem
                );
            _editorController.SelectedObject = newItem;
        }

        private void OnCanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Clipboard.ContainsData(nameof(MapObject)))
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }
    }
}