using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MapMaker.File
{
    public partial class MapObjectView : UserControl
    {
        public MapObjectView()
        {
            InitializeComponent();
        }

        public MapObject MapObject => (MapObject) DataContext;
        
    }
}