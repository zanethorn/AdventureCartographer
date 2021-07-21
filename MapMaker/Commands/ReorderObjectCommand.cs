using MapMaker.File;

namespace MapMaker.Commands
{
    public class ReorderObjectCommand:IMapCommand
    {
        public ReorderObjectCommand(MapObject mapObject, MapLayer layer, int newIndex)
        {
            MapObject = mapObject;
            Layer = layer;
            NewIndex = newIndex;
        }
        
        public MapObject MapObject { get; }
        
        public MapLayer Layer { get; }
        
        public int OldIndex { get; private set; }
        public int NewIndex { get; private set; }
        
        public void Do(MapController controller)
        {
            OldIndex = Layer.MapObjects.IndexOf(MapObject);
            Layer.MapObjects.Move(OldIndex, NewIndex);
        }

        public void Undo(MapController controller)
        {
            Layer.MapObjects.Move(NewIndex, OldIndex);
        }

        public IMapCommand Update(IMapCommand command)
        {
            if (command is ReorderObjectCommand orderCommand)
            {
                if (orderCommand.MapObject == MapObject)
                {
                    NewIndex = orderCommand.NewIndex;
                    return this;
                }
            }

            return command;
        }
    }
}