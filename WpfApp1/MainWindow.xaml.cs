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

// https://github.com/xceedsoftware/wpftoolkit EXTENDED WPF TOOLS KIT

namespace WpfApp1 {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        // this.KeyPreview = true;//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        public ColorARGB mcolor { get; set; }
        public Color clr { get; set; }
        public MainWindow() {
            InitializeComponent();
            InkCanvas1.Cursor = Cursors.Cross;

            mcolor = new ColorARGB();
            mcolor.A = 255; mcolor.R = mcolor.B = mcolor.G = 0;
            clr = Color.FromArgb(255, 0, 0, 0);
            InkCanvas1.DefaultDrawingAttributes.Color = clr;
        }

        // Отслеживания нажатия на клавиатуру
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.Key == Key.Z) {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                    MessageBox.Show("ctrl+z"); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
            }
            if (e.Key == Key.Y) {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                    MessageBox.Show("ctrl+y"); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
            }
            if (e.Key == Key.OemPlus) {
                InkWidth.Value++;
            }
            if (e.Key == Key.OemMinus) {
                InkWidth.Value--;
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
        public void Undo(object sender, RoutedEventArgs e) {
            int count = InkCanvas1.Strokes.Count;
            if (count > 0) InkCanvas1.Strokes.RemoveAt(InkCanvas1.Strokes.Count - 1);
        }
        // Кнопка вернуть
        public void Redo(object sender, RoutedEventArgs e) {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
        // Очистка доски
        private void ClearCanvas(object sender, RoutedEventArgs e) {
            InkCanvas1.Strokes.Clear();
        }

        // Закрытие приложения
        private void CloseApp(object sender, RoutedEventArgs e) {
            this.Close();
        }

        // Сохраняем свое творчество
        private void SaveCanvas(object sender, RoutedEventArgs e) {

            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "png files|*.png";
            SFD.ShowDialog();
            string Patch = SFD.FileName;

            // https://stackoverflow.com/questions/21411878/saving-a-canvas-to-png-c-sharp-wpf

            Rect bounds = VisualTreeHelper.GetDescendantBounds(InkCanvas1);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen()) {
                VisualBrush vb = new VisualBrush(InkCanvas1);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try {
                MemoryStream ms = new MemoryStream();
                pngEncoder.Save(ms);
                ms.Close();
                File.WriteAllBytes(Patch, ms.ToArray());
            }
            catch (Exception err) {
                // MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Меняем толщину кисти
        private void NumericLimit(object sender, RoutedPropertyChangedEventArgs<object> e) {
            try {
                InkCanvas1.DefaultDrawingAttributes.Width = (int)InkWidth.Value;
                InkCanvas1.DefaultDrawingAttributes.Height = (int)InkWidth.Value;
            } catch (Exception err) {
                InkWidth.Value = 1;
            }
            
        }
        // Выбираем ластик
        private void ChoiceEraser(object sender, RoutedEventArgs e) {
            InkCanvas1.DefaultDrawingAttributes.Color = Color.FromArgb(255,255,255,255);
            InkCanvas1.UseCustomCursor = true;
        }
        // Выбираем кисть
        private void ChoicePen(object sender, RoutedEventArgs e) {
            InkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            InkCanvas1.DefaultDrawingAttributes.Color = clr;
            InkCanvas1.UseCustomCursor = false;
        }
        // Выбираем "Выбрать"
        private void ChoiceSelect(object sender, RoutedEventArgs e) {
            InkCanvas1.EditingMode = InkCanvasEditingMode.Select;
            InkCanvas1.UseCustomCursor = false;
        }
        // Временный объект
        private void ChoiceTemp(object sender, RoutedEventArgs e) {
            InkCanvas1.EditingMode = InkCanvasEditingMode.GestureOnly;
            InkCanvas1.DefaultDrawingAttributes.Color = clr;
            InkCanvas1.UseCustomCursor = false;
        }

        // Отслеживаем координаты мыши
        private void MouseMove1(object sender, MouseEventArgs e) {
            TextBlock1.Text = "X = " + e.GetPosition(null).X.ToString() + " Y = " + e.GetPosition(null).Y.ToString();
        }

        private void MouseUp1(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Hi");
        }
        // Раскрываем colorCanvas для выбора цвета
        private void ColorPicker(object sender, RoutedEventArgs e)
        {
            ColorPicker1.Visibility = Visibility;
        }

        private void PickerHide(object sender, MouseEventArgs e)
        {
            ColorPicker1.Visibility = Visibility.Hidden;

            mcolor.A = Convert.ToByte(ColorPicker1.A);
            mcolor.R = Convert.ToByte(ColorPicker1.R);
            mcolor.G = Convert.ToByte(ColorPicker1.G);
            mcolor.B = Convert.ToByte(ColorPicker1.B);

            clr = Color.FromArgb(mcolor.A, mcolor.R, mcolor.G, mcolor.B);

            InkCanvas1.DefaultDrawingAttributes.Color = clr;
        }
    }

    // Класс для определения цветов
    public class ColorARGB {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
