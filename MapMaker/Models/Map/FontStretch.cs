using System.ComponentModel;
using MapMaker.Converters;

namespace MapMaker.Models.Map
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FontStretch
    {
        [Description("Ultra Condensed")]
        UltraCondensed =1,
        
        [Description("Extra Condensed")]
        ExtraCondensed=2,
        
        [Description("Condensed")]
        Condensed=3,
        
        [Description("Semi Condensed")]
        SemiCondensed=4,
        
        [Description("Normal")]
        Normal=5,
        
        [Description("Semi Expanded")]
        SemiExpanded=6,
        
        [Description("Expanded")]
        Expanded=7,
        
        [Description("Extra Expaneded")]
        ExtraExpanded=8,
        
        [Description("Ultra Expanded")]
        UltraExpanded=9
    }
}