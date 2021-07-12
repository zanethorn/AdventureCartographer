using System.Collections.Generic;

namespace MapMaker.Library
{
    public class ImageFile
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Path { get; set; }
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public int UnitWidth { get; set; }
        public int UnitHeight { get; set; }

        public IList<ImageTags> Tags { get; set; } = new List<ImageTags>();
    }
}