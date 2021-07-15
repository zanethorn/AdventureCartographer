using System.Windows.Forms.VisualStyles;
using MapMaker.Annotations;
using MapMaker.File;

namespace MapMaker
{
    public class Pointer:Tool
    {
        public Pointer(MapController controller) : base(controller, nameof(Pointer), nameof(Pointer))
        {
        }

        protected override ToolState GetToolState()
        {
            if (Controller.SelectedObject == null) return ToolState.Inactive;
            return IsDown ? ToolState.Active : ToolState.Inactive;
        }

        protected override void OnUseTool()
        {
            var offset = Position - DownPosition;
            DownPosition = Position;
            Controller.SelectedObject.OffsetX += offset.X;
            Controller.SelectedObject.OffsetY += offset.Y;
        }
    }
}