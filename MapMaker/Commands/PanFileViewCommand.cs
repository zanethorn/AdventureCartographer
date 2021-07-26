using System.Windows;
using MapMaker.Map;

namespace MapMaker.Commands
{
    public class PanFileViewCommand:IMapCommand
    {
        public PanFileViewCommand(Point newPoint)
        {
            NewPoint = newPoint;
        }
        
        public Point? OriginalPoint { get; private set; }
        public Point NewPoint { get; private set; }
        
        public void Do(MapController controller)
        {
            if (OriginalPoint == null)
            {
                OriginalPoint = controller.Offset;
            }

            controller.Offset = NewPoint;
        }

        public void Undo(MapController controller)
        {
            controller.Offset = OriginalPoint.Value;
        }

        public IMapCommand Update(IMapCommand command)
        {
            if (command is PanFileViewCommand panCommand)
            {
                NewPoint = panCommand.NewPoint;
                return this;
            }

            return command;
        }
    }
}