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
                            ),
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
            _editorController.SelectedTool.Move(e.GetPosition(FileView));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _editorController.SelectedTool.Down(e.GetPosition(FileView));
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _editorController.SelectedTool.Up(e.GetPosition(FileView));
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            
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