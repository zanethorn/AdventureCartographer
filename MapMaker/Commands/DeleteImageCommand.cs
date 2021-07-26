using MapMaker.Annotations;
using MapMaker.Map;

namespace MapMaker.Commands
{
    public class DeleteImageCommand:DeleteObjectCommand
    {
        public DeleteImageCommand([NotNull] MapImage mapObject, [NotNull] MapLayer mapLayer) : base(mapObject, mapLayer)
        {
        }
        
        
    }
}