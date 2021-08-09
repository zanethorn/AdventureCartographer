using System.ComponentModel;
using MapMaker.Converters;

namespace MapMaker.Models.Map
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FontStyle
    {
        [Description("Normal")]
        Normal,
        
        [Description("Italic")]
        Italic,
        
        [Description("Oblique")]
        Oblique
    }
}