using System.Windows.Input;

namespace MapMaker
{
    public static class CustomCommands
    {
        public static RoutedCommand Exit { get; }= new RoutedCommand(nameof(Exit), typeof(CustomCommands));

        public static RoutedCommand Copy { get; }= new RoutedCommand(nameof(Copy), typeof(CustomCommands));
        
        public static RoutedCommand ScanDirectory { get; } = new RoutedCommand(nameof(ScanDirectory), typeof(CustomCommands));

        public static RoutedCommand ViewLibraryDetails { get; } =
            new RoutedCommand(nameof(ViewLibraryDetails), typeof(CustomCommands));
        
        public static RoutedCommand ShowPreferences { get; } =
            new RoutedCommand(nameof(ShowPreferences), typeof(CustomCommands));
        
        public static RoutedCommand ShowAbout { get; } =
            new RoutedCommand(nameof(ShowAbout), typeof(CustomCommands));
    }
}