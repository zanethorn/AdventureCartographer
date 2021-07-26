using MapMaker.Map;

namespace MapMaker.Commands
{
    public class DeleteObjectCommand:IMapCommand
    {
        protected DeleteObjectCommand(MapObject mapObject, MapLayer mapLayer)
        {
            MapObject = mapObject;
            MapLayer = mapLayer;
        }
        
        public MapObject MapObject { get; }
        
        public MapLayer MapLayer { get; }
        
        public virtual void Do(MapController controller)
        {
            MapLayer.MapObjects.Remove(MapObject);
            if (controller.SelectedObject == MapObject)
            {
                controller.SelectObject(null, true);
            }
        }

        public virtual void Undo(MapController controller)
        {
            MapLayer.MapObjects.Add(MapObject);
        }

        public IMapCommand Update(IMapCommand command)
        {
            return command;
        }
    }
}