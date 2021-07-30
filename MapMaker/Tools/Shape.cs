using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MapMaker.Controllers;
using MapMaker.Models.Map;

namespace MapMaker.Tools
{
    public class Shape:ToolBase
    {
        private MapShape? _newShape;
        
        public Shape() : base("ShapePolygonPlus")
        {
        }


        protected override void OnUp(Point position)
        {
            Cursor = Cursors.IBeam;
            if (_newShape == null) return;
            var moveOffset = Position - position;
            MapController.MoveResizeObject(
                _newShape, 
                EditorController.SnapToGrid(_newShape.Offset - moveOffset),
                EditorController.SnapToGrid(_newShape.Size)
            );
            _newShape = null;
        }

        protected override void OnDown(Point position)
        {
            Cursor = Cursors.SizeAll;
            _newShape = new MapShape()
            {
                Offset = position,
                Size = new Size(0,0),
                FillBrush = (MapBrush)EditorController.DefaultBackgroundBrush.Clone(),
                StrokeBrush = (MapBrush)EditorController.DefaultForegroundBrush.Clone()
            };
            MapController.AddObjectToLayer(
                EditorController.SelectedMap,
                EditorController.SelectedLayer,
                _newShape
            );
            EditorController.SelectedObject = _newShape;
        }

        protected override void OnMove(Point position)
        {
            if (!IsDown) return;
            if (_newShape == null) return;
            var moveOffset = Position - position;
            var x = _newShape.Offset.X;
            var y = _newShape.Offset.Y;
            var width = _newShape.Size.Width - moveOffset.X;
            var height = _newShape.Size.Height - moveOffset.Y;
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
                _newShape,
                new Point(x,y),
                new Size(width, height)
            );
        }


        
    }
}