using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace MapMaker
{
    public partial class SplashDialog : Window
    {
        public SplashDialog()
        {
            InitializeComponent();
            
            DataContext = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
        }

		private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            DialogResult = true;
		}
	}
}