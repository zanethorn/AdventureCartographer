using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using MapMaker.Controllers;

namespace MapMaker
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
    {
        private EditorController? _editorController;
        private LibraryController? _libraryController;
        private MapController? _mapController;
        private SettingsController? _settingsController;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Task.Run(async () =>
            //{
                _settingsController = (SettingsController) FindResource(nameof(SettingsController))!;
                _libraryController = (LibraryController) FindResource(nameof(LibraryController))!;
                _mapController = (MapController) FindResource(nameof(MapController))!;
                _editorController = (EditorController) FindResource(nameof(EditorController))!;

                _settingsController.Init().Wait();
                _libraryController.Init(_settingsController.Settings.DefaultLibraryName).Wait();
                _mapController.Init().Wait();
                _editorController.Init().Wait();
            //});
        }
    }
}