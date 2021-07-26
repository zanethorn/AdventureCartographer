using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using MapMaker.Models;
using MapMaker.Models.Map;
using MonitoredUndo;

namespace MapMaker.Controllers
{
    public class MapController : SmartObject
    {
        private const string KeyFileName = "map.xml";
        private const string ImageDirectory = "Resources/Images/";


        public MapController()
        {
            NewMap();
        }


        public Task Init()
        {
            return Task.CompletedTask;
        }


        public MapFile NewMap()
        {
            var map = new MapFile();
            map.Layers.Add(new MapLayer {Name = "UntitledLayer_1"});
            return map;
        }

        public async Task<MapFile> LoadMap(string filename, CancellationToken cancellationToken = default)
        {
            using var zip = ZipFile.Open(filename, ZipArchiveMode.Read);

            // write the basic map.xml key file
            var keyEntry = zip.GetEntry(KeyFileName) ?? zip.CreateEntry(KeyFileName);
            await using Stream keyStream = keyEntry.Open();
            var serializer = new XmlSerializer(typeof(MapFile));
            var mapFile = (MapFile) serializer.Deserialize(keyStream)!;
            keyStream.Close();

            foreach (var image in mapFile.ImageFiles)
            {
                var entryName = $"{ImageDirectory}{image.Id}.{image.FileExtension}";
                var imgEntry = zip.GetEntry(entryName)!;
                var outStream = imgEntry.Open();
                var memStream = new MemoryStream();
                await outStream.CopyToAsync(memStream, cancellationToken);
                memStream.Seek(0, SeekOrigin.Begin);
                outStream.Close();

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memStream;
                bitmap.EndInit();
                if (bitmap.CanFreeze)
                    bitmap.Freeze();
                image.Bitmap = bitmap;
            }

            foreach (var layer in mapFile.Layers)
            foreach (var obj in layer.MapObjects)
                if (obj is MapImage mapImage)
                    mapImage.Image = mapFile.ImageFiles.Single(i => i.Id == mapImage.Image.Id);

            return mapFile;
        }

        public async Task SaveMap(string filename, MapFile mapFile, CancellationToken cancellationToken = default)
        {
            using var zip = ZipFile.Open(filename, ZipArchiveMode.Update);

            // write the basic map.xml key file
            var keyEntry = zip.GetEntry(KeyFileName) ?? zip.CreateEntry(KeyFileName);
            await using Stream keyStream = keyEntry.Open();
            var serializer = new XmlSerializer(typeof(MapFile));
            serializer.Serialize(keyStream, mapFile);
            await keyStream.FlushAsync(cancellationToken);

            // Store off all images
            foreach (var image in mapFile.ImageFiles)
            {
                var entryName = $"{ImageDirectory}{image.Id}.{image.FileExtension}";
                if (zip.GetEntry(entryName) == null)
                {
                    if (image.Bitmap.StreamSource == null)
                    {
                        zip.CreateEntryFromFile(image.Path, entryName);
                    }
                    else
                    {
                        var imgEntry = zip.CreateEntry(entryName);
                        await using var outStream = imgEntry.Open();
                        await image.Bitmap.StreamSource.CopyToAsync(outStream, cancellationToken);
                        await outStream.FlushAsync(cancellationToken);
                    }
                }
            }
        }

        public void AddLayer(MapFile mapFile, MapLayer mapLayer, int index = -1)
        {
            using (new UndoBatch(mapFile, $"Add {mapLayer}", false))
            {
                if (index == -1)
                    mapFile.Layers.Add(mapLayer);
                else
                    mapFile.Layers.Insert(index, mapLayer);
            }
        }

        public void DeleteLayer(MapFile mapFile, MapLayer mapLayer)
        {
            using (new UndoBatch(mapFile, $"Delete {mapLayer}", false))
            {
                mapFile.Layers.Remove(mapLayer);
            }
        }

        public void AddObjectToLayer(MapFile mapFile, MapLayer mapLayer, MapObject mapObject)
        {
            using (new UndoBatch(mapFile, $"Add {mapObject}", false))
            {
                mapLayer.MapObjects.Add(mapObject);

                if (mapObject is MapImage mapImage)
                {
                    var linkedBitmap = mapFile.ImageFiles.SingleOrDefault(f => f.Id == mapImage.Image.Id);
                    if (linkedBitmap == null)
                    {
                        linkedBitmap = mapImage.Image;
                        mapFile.ImageFiles.Add(linkedBitmap);
                    }

                    mapImage.Image = linkedBitmap;
                }
            }
        }

        public void DeleteObjectFromLayer(MapFile mapFile, MapLayer mapLayer, MapObject mapObject)
        {
            using (new UndoBatch(mapFile, $"Delete {mapObject}", false))
            {
                mapLayer.MapObjects.Remove(mapObject);
            }
        }

        public void MoveObjectUp(MapFile mapFile, MapLayer mapLayer, MapObject mapObject)
        {
            using (new UndoBatch(mapFile, "Reorder Object", true))
            {
                var oldIndex = mapLayer.MapObjects.IndexOf(mapObject);
                mapLayer.MapObjects.Move(oldIndex, oldIndex + 1);
            }
        }

        public void MoveObjectTop(MapFile mapFile, MapLayer mapLayer, MapObject mapObject)
        {
            using (new UndoBatch(mapFile, "Reorder Object", true))
            {
                var oldIndex = mapLayer.MapObjects.IndexOf(mapObject);
                mapLayer.MapObjects.Move(oldIndex, mapLayer.MapObjects.Count - 1);
            }
        }

        public void MoveObjectBottom(MapFile mapFile, MapLayer mapLayer, MapObject mapObject)
        {
            using (new UndoBatch(mapFile, "Reorder Object", true))
            {
                var oldIndex = mapLayer.MapObjects.IndexOf(mapObject);
                mapLayer.MapObjects.Move(oldIndex, 0);
            }
        }

        public void MoveObjectDown(MapFile mapFile, MapLayer mapLayer, MapObject mapObject)
        {
            using (new UndoBatch(mapFile, "Reorder Object", true))
            {
                var oldIndex = mapLayer.MapObjects.IndexOf(mapObject);
                mapLayer.MapObjects.Move(oldIndex, oldIndex - 1);
            }
        }

        public void MoveLayerUp(MapFile mapFile, MapLayer mapLayer)
        {
            using (new UndoBatch(mapFile, "Reorder Layer", true))
            {
                var oldIndex = mapFile.Layers.IndexOf(mapLayer);
                mapLayer.MapObjects.Move(oldIndex, oldIndex + 1);
            }
        }

        public void MoveLayerDown(MapFile mapFile, MapLayer mapLayer)
        {
            using (new UndoBatch(mapFile, "Reorder Layer", true))
            {
                var oldIndex = mapFile.Layers.IndexOf(mapLayer);
                mapLayer.MapObjects.Move(oldIndex, oldIndex - 1);
            }
        }

        public void MoveLayerTop(MapFile mapFile, MapLayer mapLayer)
        {
            using (new UndoBatch(mapFile, "Reorder Layer", true))
            {
                var oldIndex = mapFile.Layers.IndexOf(mapLayer);
                mapLayer.MapObjects.Move(oldIndex, mapFile.Layers.Count - 1);
            }
        }

        public void MoveLayerBottom(MapFile mapFile, MapLayer mapLayer)
        {
            using (new UndoBatch(mapFile, "Reorder Layer", true))
            {
                var oldIndex = mapFile.Layers.IndexOf(mapLayer);
                mapLayer.MapObjects.Move(oldIndex, 0);
            }
        }
    }
}