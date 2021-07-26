using System.Windows;
using MapMaker.Annotations;
using MapMaker.Commands;
using MapMaker.Map;

namespace MapMaker
{
    public class Shape:Tool
    {
        public Shape([NotNull] MapController controller) : base(controller, nameof(Shape), nameof(Shape))
        {
        }

        protected override ToolState GetToolState()
        {
            return ToolState.Active;
        }

        protected override void OnUseTool()
        {
            var newShape = new MapShape()
            {
                Size = new Size(100,100),
                Offset = Position
            };
            var command = new AddObjectCommand(newShape, Controller.SelectedLayer);
            Controller.IngestCommand(command);
        }
    }
}