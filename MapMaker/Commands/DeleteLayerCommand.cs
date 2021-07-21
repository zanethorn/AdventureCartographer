using MapMaker.File;

namespace MapMaker.Commands
{
    public class DeleteLayerCommand:IMapCommand
    {
        private int _index;
        
        public DeleteLayerCommand(MapLayer layer)
        {
            Layer = layer;
        }

        

        public MapLayer Layer { get; }
        
        public void Do(MapController controller)
        {
            _index = controller.MapFile.Layers.IndexOf(Layer);
            controller.MapFile.Layers.Remove(Layer);
        }

        public void Undo(MapController controller)
        {
            controller.MapFile.Layers.Insert(_index, Layer);
        }

        public IMapCommand Update(IMapCommand command)
        {
            return command;
        }
    }
}