using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MapMaker.Tools
{
    public interface ITool: INotifyPropertyChanged
    {
        Cursor Cursor { get; }
        
        BitmapImage Icon { get; }
        
        Point Position { get; }

        void Up(Point position);

        void Down(Point position);

        void Move(Point position);
    }
}