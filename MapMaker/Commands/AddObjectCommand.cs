using MapMaker.File;

namespace MapMaker.Commands
{
    public abstract class AddObjectCommand:IMapCommand
    {
        protected AddObjectCommand(MapObject mapObject, MapLayer mapLayer)
        {
            MapObject = mapObject;
            MapLayer = mapLayer;
        }
        
        public MapObject MapObject { get; }
        
        public MapLayer MapLayer { get; }
        
        public virtual void Do(MapController controller)
        {
            MapLayer.MapObjects.Add(MapObject);
        }

        public virtual void Undo(MapController controller)
        {
            MapLayer.MapObjects.Remove(MapObject);
        }

        public IMapCommand Update(IMapCommand command)
        {
            return command;
        }
    }
}