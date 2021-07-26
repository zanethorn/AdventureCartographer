using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace MapMaker.Views
{
    public partial class SplashDialog : Window
    {
        public SplashDialog()
        {
            InitializeComponent();

            DataContext = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }
    }
}