using System.Xml.Serialization;

namespace MapMaker.Models.Map
{
    [XmlType(nameof(MapLayer))]
    public class MapLayer : SmartObject
    {
        private bool _isLocked;
        private bool _isVisible = true;
        private SmartCollection<MapObject> _mapObjects = new();
        private string _name = string.Empty;

        [XmlAttribute]
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (value == _isLocked) return;
                _isLocked = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
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

        [XmlArray]
        public SmartCollection<MapObject> MapObjects
        {
            get => _mapObjects;
            set
            {
                if (Equals(value, _mapObjects)) return;
                _mapObjects = value;
                OnPropertyChanged();
            }
        }

        protected override void OnClone(object clone)
        {
            base.OnClone(clone);
            var myClone = (MapLayer) clone;
            myClone._mapObjects = (SmartCollection<MapObject>) _mapObjects.Clone();
        }
    }
}