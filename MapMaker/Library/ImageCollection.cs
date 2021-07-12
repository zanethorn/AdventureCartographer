using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapMaker.Library
{
    public class ImageCollection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ObservableCollection<ImageFile> Images { get; set; } = new ObservableCollection<ImageFile>();
    }
}