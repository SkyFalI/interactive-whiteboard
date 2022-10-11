using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1 {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // this.KeyPreview = true;//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        public ColorRGB mcolor { get; set; }
        public Color clr { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            mcolor = new ColorRGB();
            mcolor.R = mcolor.B = mcolor.G = 0;
        }

        // Отслеживания нажатия на клавиатуру
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Z)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    MessageBox.Show("ctrl+z"); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
            }
            if (e.Key == Key.Y)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    MessageBox.Show("ctrl+y"); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
            }
            // Если понадобится нажатия трех клавиш
            /*ModifierKeys combCtrSh = ModifierKeys.Control | ModifierKeys.Shift;
            if (e.Key == Key.B)
            {
                if ((e.KeyboardDevice.Modifiers & combCtrSh) == combCtrSh)
                    MessageBox.Show("Ctrl+Shift+B");
            }*/
        }


        // Отменить дейсвтие
        public void Undo(object sender, RoutedEventArgs e)
        {
            int count = InkCanvas1.Strokes.Count;
            if (count > 0) InkCanvas1.Strokes.RemoveAt(InkCanvas1.Strokes.Count - 1);
        }
        // Кнопка вернуть
        public void Redo(object sender, RoutedEventArgs e)
        {
            InkCanvas1.Strokes.Add(InkCanvas1.Strokes[1]);
        }
        // Очистка доски
        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            InkCanvas1.Strokes.Clear();
        }

        // Закрытие приложения
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Меняем цвет при изменении значенний у слайдера
        private void ColorChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mcolor.R = Convert.ToByte(ColorRed.Value);
            mcolor.G = Convert.ToByte(ColorGreen.Value);
            mcolor.B = Convert.ToByte(ColorBlue.Value);

            clr = Color.FromRgb(mcolor.R, mcolor.G, mcolor.B);

            InkCanvas1.DefaultDrawingAttributes.Color = clr;
        }

        // Сохраняем свое творчество
        private void SaveCanvas(object sender, RoutedEventArgs e)
        {

            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "png files|*.png";
            SFD.ShowDialog();
            string Patch = SFD.FileName;
            // https://stackoverflow.com/questions/21411878/saving-a-canvas-to-png-c-sharp-wpf

            Rect bounds = VisualTreeHelper.GetDescendantBounds(InkCanvas1);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(InkCanvas1);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                MemoryStream ms = new MemoryStream();
                pngEncoder.Save(ms);
                ms.Close();
                File.WriteAllBytes(Patch, ms.ToArray());
            }
            catch (Exception err)
            {
                // MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Меняем толщину кисти
        private void NumericLimit(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            InkCanvas1.DefaultDrawingAttributes.Width = (int)InkWidth.Value;
            InkCanvas1.DefaultDrawingAttributes.Height = (int)InkWidth.Value;
        }
        // Выбираем ластик
        private void ChoiceEraser(object sender, RoutedEventArgs e)
        {
            InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }
        // Выбираем кисть
        private void ChoicePen(object sender, RoutedEventArgs e)
        {
            InkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }
        // Выбираем "Выбрать"
        private void ChoiceSelect(object sender, RoutedEventArgs e)
        {
            InkCanvas1.EditingMode = InkCanvasEditingMode.Select;
        }
        // Временный объект
        private void ChoiceTemp(object sender, RoutedEventArgs e)
        {
            InkCanvas1.EditingMode = InkCanvasEditingMode.GestureOnly;
        }

    }

    // Класс для определения цветов
    public class ColorRGB
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
