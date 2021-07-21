using MapMaker.File;

namespace MapMaker.Commands
{
    public class AddLayerCommand:IMapCommand
    {
        public AddLayerCommand(MapLayer layer, int index=-1)
        {
            Layer = layer;
            Index = index;
        }

        public int Index { get; }

        public MapLayer Layer { get; }
        
        public void Do(MapController controller)
        {
            if (Index == -1)
            {
                controller.MapFile.Layers.Add(Layer);
            }
            else
            {
                controller.MapFile.Layers.Insert(Index, Layer);
            }
        }

        public void Undo(MapController controller)
        {
            controller.MapFile.Layers.Remove(Layer);
        }

        public IMapCommand Update(IMapCommand command)
        {
            return command;
        }
    }
}