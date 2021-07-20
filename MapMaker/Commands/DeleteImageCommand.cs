using MapMaker.Annotations;
using MapMaker.File;

namespace MapMaker.Commands
{
    public class DeleteImageCommand:DeleteObjectCommand
    {
        public DeleteImageCommand([NotNull] MapImage mapObject, [NotNull] MapLayer mapLayer) : base(mapObject, mapLayer)
        {
        }
        
        
    }
}