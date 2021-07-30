using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MapMaker.Controllers;

namespace MapMaker.Tools
{
    public class Pointer:ToolBase
    {
        
        public Pointer() : base("CursorDefault")
        {
        }

        protected override void OnUp(Point position)
        {
            Cursor = Cursors.Arrow;
            if (EditorController.SelectedObject == null) return;
            var moveOffset = Position - position;
            MapController.MoveResizeObject(
                EditorController.SelectedObject, 
                EditorController.SnapToGrid(EditorController.SelectedObject.Offset - moveOffset),
                EditorController.SelectedObject.Size
            );
        }

        protected override void OnDown(Point position)
        {
            Cursor = Cursors.ScrollAll;
        }

        protected override void OnMove(Point position)
        {
            if (!IsDown) return;
            var moveOffset = Position - position;
            if (EditorController.SelectedObject != null)
            {
                MapController.MoveResizeObject(
                    EditorController.SelectedObject,
                    EditorController.SelectedObject.Offset - moveOffset,
                    EditorController.SelectedObject.Size
                );
            }
        }

        
    }
}