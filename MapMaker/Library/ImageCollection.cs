using System.Collections.Generic;

namespace MapMaker.Library
{
    public class ImageCollection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public IList<ImageFile> Files { get; set; } = new List<ImageFile>();
    }
}