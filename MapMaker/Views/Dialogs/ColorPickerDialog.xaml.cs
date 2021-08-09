using System.Windows;
using System.Windows.Media;

namespace MapMaker.Views.Dialogs
{
    public partial class ColorPickerDialog : Window
    {
        public ColorPickerDialog()
        {
            InitializeComponent();
        }

        public Color Color
        {
            get => ColorCanvas.SelectedColor.GetValueOrDefault();
            set => ColorCanvas.SelectedColor = value;
        }

        private void OnDefault(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}