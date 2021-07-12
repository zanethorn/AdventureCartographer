using System.Windows.Input;

namespace MapMaker
{
    public static class CustomCommands
    {
        public static RoutedCommand Exit { get; }= new RoutedCommand(nameof(Exit), typeof(CustomCommands));

        public static RoutedCommand ScanDirectory { get; } = new RoutedCommand(nameof(ScanDirectory), typeof(CustomCommands));

        public static RoutedCommand ViewLibraryDetails { get; } =
            new RoutedCommand(nameof(ViewLibraryDetails), typeof(CustomCommands));
    }
}