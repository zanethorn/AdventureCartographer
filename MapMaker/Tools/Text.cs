using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MapMaker.Controllers;
using MapMaker.Models.Map;

namespace MapMaker.Tools
{
    public class Text:ToolBase
    {
        private MapText? _newText;
        
        
        public Text() : base("CursorText")
        {
        }

        protected override void OnUp(Point position)
        {
            Cursor = Cursors.IBeam;
            if (_newText == null) return;
            var moveOffset = Position - position;
            MapController.MoveResizeObject(
                _newText, 
                EditorController.SnapToGrid(_newText.Offset - moveOffset),
                EditorController.SnapToGrid(_newText.Size)
            );
            _newText = null;
        }

        protected override void OnDown(Point position)
        {
            Cursor = Cursors.SizeAll;
            _newText = new MapText()
            {
                Offset = position,
                Size = new Size(0,0),
                FillBrush = (MapBrush)EditorController.DefaultBackgroundBrush.Clone()
            };
            MapController.AddObjectToLayer(
                EditorController.SelectedMap,
                EditorController.SelectedLayer,
                _newText
            );
            EditorController.SelectedObject = _newText;
        }

        protected override void OnMove(Point position)
        {
            if (!IsDown) return;
            if (_newText == null) return;
            var moveOffset = Position - position;
            var x = _newText.Offset.X;
            var y = _newText.Offset.Y;
            var width = _newText.Size.Width - moveOffset.X;
            var height = _newText.Size.Height - moveOffset.Y;
            if (width < 0)
            {
                x += width;
                width = -width;
            }

            if (height < 0)
            {
                y += height;
                height = -height;
            }
            MapController.MoveResizeObject(
                _newText,
                new Point(x,y),
                new Size(width, height)
            );
        }

        
    }
}