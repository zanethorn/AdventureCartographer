using System.ComponentModel;
using MapMaker.Converters;

namespace MapMaker.Models.Map
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum BrushTypes
    {
        [Description("Solid Color")]
        Solid,
        
        [Description("Linear Gradient")]
        LinearGradient,
        
        [Description("RadialGradient")]
        RadialGradient,
        
        [Description("Image Brush")]
        Image,
        
        [Description("Object Painter")]
        Object
    }
}