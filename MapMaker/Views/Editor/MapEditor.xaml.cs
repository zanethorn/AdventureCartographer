using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MapMaker.Controllers;
using MapMaker.Models.Library;
using MapMaker.Models.Map;
using MonitoredUndo;

namespace MapMaker.Views.Editor
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
        private Point _lastScreenPosition;

        public MapEditor()
        {
            InitializeComponent();

            _libraryController = (LibraryController) FindResource(nameof(LibraryController));
            _mapController = (MapController) FindResource(nameof(MapController));
            _editorController = (EditorController) FindResource(nameof(EditorController));
            _settingsController = (SettingsController) FindResource(nameof(SettingsController));
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
            var screenPosition = e.GetPosition(this);
            var thisPosition = e.GetPosition(FileView);
            _editorController.CursorPosition = thisPosition;
            Cursor cursor = Cursors.Arrow;
            
            void Pan()
            {
                var screenOffset = _lastScreenPosition - screenPosition;
                cursor = Cursors.ScrollAll;
                _editorController.Offset -= screenOffset;
            }
            var moveOffset = _lastPosition -thisPosition;

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
                            Pan();
                        }
                    }
                        break;
                    case ToolTypes.Pan:
                    {
                        Pan();
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
            _lastScreenPosition = screenPosition;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _downPosition = e.GetPosition(FileView);
            _editorController.CursorPosition = _downPosition;

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

		private void IgnoreMouse(object sender, MouseEventArgs e)
		{
            e.Handled = true;
		}
	}
}