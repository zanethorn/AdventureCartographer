using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MapMaker.Controllers;
using MapMaker.Models.Map;

namespace MapMaker.Tools
{
    public class Text:CreateAndSizeTool
    {
        private MapText? _newText;
        
        
        public Text() : base("CursorText")
        {
        }

        protected override void OnUp(Point position)
        {
            Cursor = Cursors.IBeam;
            base.OnUp(position);
        }

        protected override void OnDown(Point position)
        {
            Cursor = Cursors.SizeAll;
            base.OnDown(position);
        }

        protected override MapObject CreateMapObject(Point position)
        {
            return new MapText()
            {
                Offset = position,
                Size = new Size(0,0),
                FillBrush = (MapBrush)EditorController.DefaultBackgroundBrush.Clone()
            };
        }


        
    }
}