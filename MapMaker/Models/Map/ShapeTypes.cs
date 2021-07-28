using System.ComponentModel;
using MapMaker.Converters;

namespace MapMaker.Models.Map
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ShapeTypes
    {
        Rectangle,
        Ellipse,
        Polygon,
        Star
    }
}