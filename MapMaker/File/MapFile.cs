using System.Collections.Generic;
using MapMaker.Library;

namespace MapMaker.File
{
    public class MapFile
    {
        public string Name { get; set; }
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public int GridCellWidth { get; set; }
        public int GridCellHeight { get; set; }

        public IList<ImageFile> SourceFiles { get; set; } = new List<ImageFile>();
    }
}