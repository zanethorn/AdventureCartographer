using System.ComponentModel;
using System.Runtime.CompilerServices;
using MapMaker.Annotations;

namespace MapMaker.File
{
    public abstract class MapObject: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}