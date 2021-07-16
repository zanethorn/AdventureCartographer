using System;
using System.Collections.Generic;

namespace MapMaker.Library
{
    public class ImageFile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Path { get; set; }
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        
        
        
        public string FileExtension { get; set; }
        
        public long FileSize { get; set; }

        public IList<ImageTags> Tags { get; set; } = new List<ImageTags>();
    }
}