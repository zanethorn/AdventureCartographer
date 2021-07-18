using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MapMaker.Library
{
    public class ImageFile
    {
        [XmlAttribute]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [XmlAttribute]
        public string FullName { get; set; }
        
        [XmlAttribute]
        public string ShortName { get; set; }
        
        [XmlAttribute]
        public string Path { get; set; }
        
        [XmlIgnore]
        public int PixelWidth { get; set; }
        
        [XmlIgnore]
        public int PixelHeight { get; set; }
        
        
        [XmlIgnore]
        public string FileExtension { get; set; }
        
        [XmlIgnore]
        public long FileSize { get; set; }

        [XmlIgnore]
        public IList<ImageTags> Tags { get; set; } = new List<ImageTags>();
    }
}