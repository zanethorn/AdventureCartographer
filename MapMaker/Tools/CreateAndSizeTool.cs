using System.Windows;
using MapMaker.Models.Map;

namespace MapMaker.Tools
{
    public abstract class CreateAndSizeTool:ToolBase
    {
        public CreateAndSizeTool(string iconName) : base(iconName)
        {
        }
        
        protected MapObject? NewObject { get; private set; }

        protected override void OnUp(Point position)
        {
            if (NewObject == null) return;
            var moveOffset = Position - position;
            MapController.MoveResizeObject(
                NewObject, 
                EditorController.SnapToGrid(NewObject.Offset - moveOffset),
                EditorController.SnapToGrid(NewObject.Size)
            );
            NewObject = null;
        }

        protected override void OnDown(Point position)
        {
            NewObject = CreateMapObject(position);
            MapController.AddObjectToLayer(
                EditorController.SelectedMap,
                EditorController.SelectedLayer,
                NewObject
            );
            EditorController.SelectedObject = NewObject;
        }

        protected override void OnMove(Point position)
        {
            if (!IsDown) return;
            if (NewObject == null) return;
            var moveOffset = Position - position;
            var x = NewObject.Offset.X;
            var y = NewObject.Offset.Y;
            var width = NewObject.Size.Width - moveOffset.X;
            var height = NewObject.Size.Height - moveOffset.Y;
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
                NewObject,
                new Point(x,y),
                new Size(width, height)
            );
        }

        protected abstract MapObject CreateMapObject(Point position);
    }
}