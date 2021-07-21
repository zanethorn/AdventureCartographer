using System.Windows.Input;

namespace MapMaker
{
    public static class CustomCommands
    {
        public static RoutedCommand Export { get; }= new(nameof(Export), typeof(CustomCommands));
        
        public static RoutedCommand ExportAs { get; }= new(nameof(ExportAs), typeof(CustomCommands));
        
        public static RoutedCommand Exit { get; }= new(nameof(Exit), typeof(CustomCommands));

        
        
        public static RoutedCommand ScanDirectory { get; } = new(nameof(ScanDirectory), typeof(CustomCommands));

        public static RoutedCommand ViewLibraryDetails { get; } =
            new(nameof(ViewLibraryDetails), typeof(CustomCommands));
        
        public static RoutedCommand ShowPreferences { get; } =
            new(nameof(ShowPreferences), typeof(CustomCommands));
        
        public static RoutedCommand ShowAbout { get; } =
            new(nameof(ShowAbout), typeof(CustomCommands));
        
        public static RoutedCommand NewLayer { get; }= new(nameof(NewLayer), typeof(CustomCommands));
        
        public static RoutedCommand MoveUp { get; }= new(nameof(MoveUp), typeof(CustomCommands));
        
        public static RoutedCommand MoveToTop { get; }= new(nameof(MoveToTop), typeof(CustomCommands));
        
        public static RoutedCommand MoveDown { get; }= new(nameof(MoveDown), typeof(CustomCommands));
        
        public static RoutedCommand MoveToBottom { get; }= new(nameof(MoveToBottom), typeof(CustomCommands));
        
        public static RoutedCommand Duplicate { get; }= new(nameof(Duplicate), typeof(CustomCommands));
        
        public static RoutedCommand Merge { get; }= new(nameof(Merge), typeof(CustomCommands));
    }
}