using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MapMaker.Controllers;
using MapMaker.Models.Map;

namespace MapMaker.Tools
{
    public class Shape : CreateAndSizeTool
    {
        private MapShape? _newShape;

        public Shape() : base("ShapePolygonPlus")
        {
        }


        protected override void OnUp(Point position)
        {
            Cursor = Cursors.Cross;
            base.OnUp(position);
        }

        protected override void OnDown(Point position)
        {
            Cursor = Cursors.SizeAll;
            base.OnDown(position);
        }


        protected override MapObject CreateMapObject(Point position)
        {
            return new MapShape()
            {
                Offset = position,
                Size = new Size(0, 0),
                FillBrush = (MapBrush) EditorController.DefaultBackgroundBrush.Clone(),
                StrokeBrush = (MapBrush) EditorController.DefaultForegroundBrush.Clone()
            };
        }
    }
}