using System.ComponentModel;
using MapMaker.Converters;

namespace MapMaker.Models.Map
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FontWeight
    {
        [Description("Thin")]
        Thin = 100,

        [Description("Extra Light")]
        ExtraLight = 200,

        [Description("Light")]
        Light = 300,

        [Description("Normal")]
        Normal = 400,

        [Description("Medium")]
        Medium = 500,

        [Description("Semi Bold")]
        SemiBold = 600,

        [Description("Bold")]
        Bold = 700,

        [Description("Extra Bold")]
        ExtraBold = 800,

        [Description("Black")]
        Black = 900,

        [Description("Ultra Black")]
        UltraBlack = 950
    }
}