using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1 {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public ColorRGB mcolor { get; set; }
        public Color clr { get; set; }
        public MainWindow () {
            InitializeComponent();
            mcolor = new ColorRGB();
            mcolor.R = mcolor.B = mcolor.G = 0;

        }

        private void ClearCanvas (object sender, RoutedEventArgs e) {
            InkCanvas.Strokes.Clear();
        }
        private void ColorChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            mcolor.R = Convert.ToByte(ColorRed.Value);
            mcolor.G = Convert.ToByte(ColorGreen.Value);
            mcolor.B = Convert.ToByte(ColorBlue.Value);

            clr = Color.FromRgb(mcolor.R, mcolor.G, mcolor.B);

            InkCanvas.DefaultDrawingAttributes.Color = clr;
        }
    }

    
    public class ColorRGB
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        
    }

}
