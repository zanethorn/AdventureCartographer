using MapMaker.Map;

namespace MapMaker.Commands
{
    public interface IMapCommand
    {

        void Do(MapController controller);

        void Undo(MapController controller);

        IMapCommand Update(IMapCommand command);

    }
}