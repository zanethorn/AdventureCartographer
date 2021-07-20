using System.ComponentModel;
using MapMaker.Converters;

namespace MapMaker.Library
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CollectionModes
    {
        [Description("One Per Folder")]
        PerFolder,
        
        [Description("Create New Collection")]
        NewCollection,
        
        [Description("Use Default Collection")]
        DefaultCollection
    }
}