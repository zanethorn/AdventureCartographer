using System.Windows.Input;

namespace MapMaker
{
    public static class CustomCommands
    {
        public static RoutedCommand Exit { get; }= new RoutedCommand("Exit", typeof(CustomCommands));
    }
}