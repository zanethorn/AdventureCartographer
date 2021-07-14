using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapMaker
{
    public partial class ColorPickerButton : Button
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color), typeof(Color),
                typeof(ColorPickerButton)
            );

        public ColorPickerButton()
        {
            InitializeComponent();
            
        }

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set
            {
                SetValue(ColorProperty, value);
                Background = new SolidColorBrush(value);
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorPickerDialog {Color = this.Color};
            if (dialog.ShowDialog() == true)
            {
                this.Color = dialog.Color;
            }
        }
    }
}