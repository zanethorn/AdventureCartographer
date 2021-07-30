using System.ComponentModel;

namespace MapMaker.Models.Map
{
    public interface IHasFillBrush:INotifyPropertyChanged
    {
        MapBrush FillBrush { get; set; }
    }
}