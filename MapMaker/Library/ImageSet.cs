using System.Collections.Generic;

namespace MapMaker.Library
{
    public class ImageSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public IList<ImageFile> Tags { get; set; } = new List<ImageFile>();
    }
}