using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using MapMaker.Annotations;
using MapMaker.File;

namespace MapMaker
{
    public class Pointer:Tool
    {
        public Pointer(MapController controller) : base(controller, nameof(Pointer), nameof(Pointer))
        {
            _activeCursor = Cursors.ScrollAll;
        }

        protected override ToolState GetToolState()
        {
            if (Controller.SelectedObject == null) return ToolState.Invalid;
            return IsDown ? ToolState.Active : ToolState.Inactive;
        }

        protected override void OnUseTool()
        {
            var offset = Position - DownPosition;
            Controller.SelectedObject.OffsetX += offset.X;
            Controller.SelectedObject.OffsetY += offset.Y;
            DownPosition = Position;
        }
    }
}