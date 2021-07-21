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

        public SmartCollection<ImageFile> Images { get; set; } = new();

        protected override void OnNotificationsDispatched()
        {
            Images.DispatchNotifications();
            base.OnNotificationsDispatched();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Images.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}