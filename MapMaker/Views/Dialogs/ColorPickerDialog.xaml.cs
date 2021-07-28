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
            get => Picker.SelectedColor;
            set => Picker.SelectedColor = value;
        }

        private void OnDefault(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}