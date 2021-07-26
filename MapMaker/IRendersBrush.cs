using System.Windows.Media;
using System.Xml.Serialization;

namespace MapMaker.Map
{
    public interface IRendersBrush
    {
        [XmlIgnore]
        Brush RenderedBrush => GetRenderBrush();


        Brush GetRenderBrush();
    }
}