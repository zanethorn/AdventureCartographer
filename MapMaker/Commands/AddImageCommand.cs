using System.Linq;
using MapMaker.Annotations;
using MapMaker.Map;

namespace MapMaker.Commands
{
    public class AddImageCommand:AddObjectCommand
    {
        private bool _addedBitmap;
        
        public AddImageCommand([NotNull] MapImage mapObject, [NotNull] MapLayer mapLayer) : base(mapObject, mapLayer)
        {
        }

        public override void Do(MapController controller)
        {
            var mapImage = MapObject as MapImage;
            var linkedBitmap = controller.MapFile.ImageFiles.SingleOrDefault(f => f.Id == mapImage.Image.Id);
            if (linkedBitmap == null)
            {
                linkedBitmap = mapImage.Image;
                controller.MapFile.ImageFiles.Add(linkedBitmap);
                _addedBitmap = true;
            }

            mapImage.Image = linkedBitmap;
            
            base.Do(controller);
        }

        public override void Undo(MapController controller)
        {
            base.Undo(controller);

            if (_addedBitmap)
            {
                var mapImage = MapObject as MapImage;
                var linkedBitmap = controller.MapFile.ImageFiles.SingleOrDefault(f => f.Id == mapImage.Image.Id);
                controller.MapFile.ImageFiles.Remove(linkedBitmap);
            }
        }
    }
}