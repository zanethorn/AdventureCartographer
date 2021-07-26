using System.Windows;
using MapMaker.Map;

namespace MapMaker.Commands
{
    public class DragResizeObjectCommand:IMapCommand
    {

        public DragResizeObjectCommand(MapObject mapObject, Point newOffset, Size newSize)
        {
            MapObject = mapObject;
            NewOffset = newOffset;
            NewSize = newSize;
        }
        
        public Point? OriginalOffset { get; private set; }
        public Point NewOffset { get; private set; }
        
        public Size? OriginalSize { get; private set; }
        public Size NewSize { get; private set; }

        public MapObject MapObject { get; }
        
        public void Do(MapController controller)
        {
            if (OriginalOffset == null)
            {
                OriginalOffset = MapObject.Offset;
            }

            if (OriginalSize == null)
            {
                OriginalSize = MapObject.Size;
            }

            MapObject.Offset = NewOffset;//controller.SnapToGrid(NewOffset);
            MapObject.Size = NewSize;//controller.SnapToGrid(NewSize);
        }

        public void Undo(MapController controller)
        {
            MapObject.Offset = OriginalOffset.Value;
            MapObject.Size = OriginalSize.Value;
        }

        public IMapCommand Update(IMapCommand command)
        {
            if (command is DragResizeObjectCommand dragCommand && dragCommand.MapObject == this.MapObject)
            {
                NewOffset = dragCommand.NewOffset;
                NewSize = dragCommand.NewSize;
                return this;
            }

            return command;
        }
    }
}