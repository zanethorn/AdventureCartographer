using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MapMaker.File
{
    public static class MapLoader
    {
        private const string KEY_FILE_NAME = "map.xml";
        private const string IMAGE_DIRECTORY="Resources/Images/";
        
        public static async Task Save(string filename, MapFile mapFile, CancellationToken cancellationToken = default)
        {
            using var zip = ZipFile.Open(filename, ZipArchiveMode.Update);
            
            // write the basic map.xml key file
            var keyEntry = zip.GetEntry(KEY_FILE_NAME) ?? zip.CreateEntry(KEY_FILE_NAME);
            await using Stream keyStream = keyEntry.Open();
            var serializer = new XmlSerializer(typeof(MapFile));
            serializer.Serialize(keyStream, mapFile);
            await keyStream.FlushAsync(cancellationToken);

            // Store off all images
            foreach (var image in mapFile.ImageFiles)
            {
                var entryName = $"{IMAGE_DIRECTORY}{image.Id}.{image.FileExtension}";
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

        public static MapFile? Load(string filename)
        {
            using var zip = ZipFile.Open(filename, ZipArchiveMode.Read);
            
            // write the basic map.xml key file
            var keyEntry = zip.GetEntry(KEY_FILE_NAME) ?? zip.CreateEntry(KEY_FILE_NAME);
            using Stream keyStream = keyEntry.Open();
            var serializer = new XmlSerializer(typeof(MapFile));
            var mapFile = (MapFile)serializer.Deserialize(keyStream);
            keyStream.Close();

            foreach (var image in mapFile.ImageFiles)
            {
                var entryName = $"{IMAGE_DIRECTORY}{image.Id}.{image.FileExtension}";
                var imgEntry = zip.GetEntry(entryName);
                var outStream = imgEntry.Open();
                var memStream = new MemoryStream();
                outStream.CopyTo(memStream);
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
            {
                foreach (var obj in layer.MapObjects)
                {
                    if (obj is MapImage mapImage)
                    {
                        mapImage.Image = mapFile.ImageFiles.SingleOrDefault(i => i.Id == mapImage.Image.Id);
                    }
                }
            }

            return mapFile;
        }
    }
}