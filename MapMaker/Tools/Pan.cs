using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using MapMaker.Annotations;
using MapMaker.Commands;
using MapMaker.File;

namespace MapMaker
{
    public class Pan : Tool
    {
        public Pan([NotNull] MapController controller) : base(controller, nameof(Pan), nameof(Pan))
        {
            _activeCursor = Cursors.ScrollAll;
            _inactiveCursor = Cursors.Hand;
        }

        protected override ToolState GetToolState()
        {
            if (Position.X < 0 || Position.X > Controller.MapFile.PixelWidth || Position.Y == 0 ||
                Position.Y > Controller.MapFile.PixelHeight) return ToolState.Invalid;
            return IsDown ? ToolState.Active : ToolState.Inactive;
        }

        protected override void OnUseTool()
        {
            var offset = Position - DownPosition;
            DownPosition = Position - offset;
            var command = new PanFileViewCommand(Controller.Offset + offset);
            Controller.IngestCommand(command);
        }
    }
}