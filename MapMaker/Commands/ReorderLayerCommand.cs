using MapMaker.File;

namespace MapMaker.Commands
{
    public class ReorderLayerCommand:IMapCommand
    {
        public ReorderLayerCommand(MapLayer layer, int newIndex)
        {
            Layer = layer;
            NewIndex = newIndex;
        }
        
        public MapLayer Layer { get; }
        
        public int OldIndex { get; private set; }
        public int NewIndex { get; private set; }
        
        public void Do(MapController controller)
        {
            OldIndex = controller.MapFile.Layers.IndexOf(Layer);
            controller.MapFile.Layers.Move(OldIndex, NewIndex);
            controller.MapFile.Touch(nameof(MapFile.Layers));
        }

        public void Undo(MapController controller)
        {
            controller.MapFile.Layers.Move(NewIndex, OldIndex);
        }

        public IMapCommand Update(IMapCommand command)
        {
            if (command is ReorderLayerCommand orderCommand)
            {
                if (orderCommand.Layer == Layer)
                {
                    NewIndex = orderCommand.NewIndex;
                    return this;
                }
            }

            return command;
        }
    }
}