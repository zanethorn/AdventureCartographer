using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapMaker.Library
{
    public class ImageCollection:SmartObject
    {
        private int _id;
        private string _name;

        public int Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ImageFile> Images { get; set; } = new();
    }
}