using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

// https://github.com/xceedsoftware/wpftoolkit EXTENDED WPF TOOLS KIT

namespace WpfApp1 {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private ColorARGB mcolor { get; set; }
        private Color clr { get; set; }

        private List<StrokeCollection> Desks { get; set; } = new List<StrokeCollection>();
        private int DeskNumber = 0;

        public MainWindow()
        {
            InitializeComponent();
            inkCanvas1.Cursor = Cursors.Cross;

            mcolor = new ColorARGB();
            clr = Color.FromArgb(255, 0, 0, 0);
            inkCanvas1.DefaultDrawingAttributes.Color = clr;

            Desks.Add(inkCanvas1.Strokes.Clone());
        }

        // Отслеживания нажатия на клавиатуру
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //Изменение масштаба пера
            InkWidth.Value += Convert.ToInt32(e.Key == Key.OemPlus || e.Key == Key.Add);
            InkWidth.Value -= Convert.ToInt32(e.Key == Key.OemMinus || e.Key == Key.Subtract);
            // Быстрые действия
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.Z)
                    Undo(this, e);
                else if (e.Key == Key.Y)
                    Redo(this, e);
                else if (e.Key == Key.Left)
                    previousDesk(this, e);
                else if (e.Key == Key.Right)
                    nextDesk(this, e);
            }
            // Если понадобится нажатия трех клавиш
            ModifierKeys combCtrSh = ModifierKeys.Control | ModifierKeys.Shift;
            if (e.Key == Key.B)
            {
                if ((e.KeyboardDevice.Modifiers & combCtrSh) == combCtrSh)
                    MessageBox.Show("Ctrl+Shift+B");
            }
        }
        // Отменить дейсвтие
        public void Undo(object sender, RoutedEventArgs e)
        {
            if (inkCanvas1.Strokes.Count > 0) inkCanvas1.Strokes.RemoveAt(inkCanvas1.Strokes.Count - 1);
        }
        // Кнопка вернуть
        public void Redo(object sender, RoutedEventArgs e)
        {
            try
            {
                inkCanvas1.Strokes.Add(Desks[DeskNumber][inkCanvas1.Strokes.Count]);
            }
            catch { MessageBox.Show("Нечего возвращать"); }
        }

        // Очистка доски
        private void ClearCanvas(object sender, RoutedEventArgs e) { inkCanvas1.Strokes.Clear(); }

        // Закрытие приложения
        private void CloseApp(object sender, RoutedEventArgs e) { this.Close(); }

        // Сохраняем свое творчество
        private void SaveCanvas(object sender, RoutedEventArgs e)
        {

            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "png files|*.png";
            SFD.ShowDialog();

            //string Patch = SFD.FileName;
            // https://stackoverflow.com/questions/21411878/saving-a-canvas-to-png-c-sharp-wpf

            Rect bounds = VisualTreeHelper.GetDescendantBounds(inkCanvas1);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(inkCanvas1);
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
                File.WriteAllBytes(SFD.FileName, ms.ToArray());
            }
            catch { /*MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);*/ }
        }

        // Меняем толщину кисти
        private void NumericLimit(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                inkCanvas1.DefaultDrawingAttributes.Width = (int)InkWidth.Value;
                inkCanvas1.DefaultDrawingAttributes.Height = (int)InkWidth.Value;
            }
            catch { InkWidth.Value = 1; }

        }
        // Выбираем ластик
        private void ChoiceEraser(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            inkCanvas1.DefaultDrawingAttributes.Color = Color.FromArgb(255, 255, 255, 255);
            inkCanvas1.UseCustomCursor = true;
        }
        // Выбираем кисть
        private void ChoicePen(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            inkCanvas1.DefaultDrawingAttributes.Color = clr;
            inkCanvas1.UseCustomCursor = false;
        }
        // Выбираем "Выбрать"
        private void ChoiceSelect(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Select;
            inkCanvas1.UseCustomCursor = false;
        }
        // Временный объект
        private void ChoiceTemp(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.GestureOnly;
            inkCanvas1.DefaultDrawingAttributes.Color = clr;
            inkCanvas1.UseCustomCursor = false;
        }

        // Отслеживаем координаты мыши
        private void MouseMove1(object sender, MouseEventArgs e)
        {
            textBlock1.Text = "X = " + e.GetPosition(null).X.ToString() + " Y = " + e.GetPosition(null).Y.ToString();
        }
        // при отпускании правой кнопки мыши вызываем ToolBar где распaположена мышь
        private void MouseRightButtonUp1(object sender, MouseButtonEventArgs e)
        {
            var x = e.GetPosition(null).X;
            var y = e.GetPosition(null).Y;

            var mainheight = MainForm.Width;
            var mainwidth = MainForm.Height;
            var tbheight = test1.ActualHeight;
            var tbWidth = test1.ActualWidth;
            if (mainwidth / 2 < x)
                x -= tbWidth;
            if (mainheight / 2 < y)
                y -= tbheight;
            test1.Margin = new Thickness(x, y, 0, 0);

            test1.Visibility = Visibility;
        }
        // Раскрываем colorCanvas для выбора цвета
        private void ColorPicker(object sender, RoutedEventArgs e) { colorPicker1.Visibility = Visibility; }
        // При потери фокуса у ColorPicker смена цвета кисти
        private void PickerHide(object sender, MouseEventArgs e)
        {

            colorPicker1.Visibility = Visibility.Hidden;

            mcolor.A = colorPicker1.A;
            mcolor.R = colorPicker1.R;
            mcolor.G = colorPicker1.G;
            mcolor.B = colorPicker1.B;

            clr = Color.FromArgb(mcolor.A, mcolor.R, mcolor.G, mcolor.B);

            inkCanvas1.DefaultDrawingAttributes.Color = clr;
        }
        // Клонируем inkCanvas1 в temp для UnDo/ReDo
        private void MouseLeftButtonUp1(object sender, MouseButtonEventArgs e)
        {
            Desks[DeskNumber] = inkCanvas1.Strokes.Clone();
        }
        // Убираем ToolBar
        private void StackPanelHide(object sender, MouseEventArgs e)
        {
            test1.Visibility = Visibility.Hidden;
        }
        // Меняем доску на следующую
        private void nextDesk(object sender, RoutedEventArgs e)
        {
            DeskNumber++;
            if (DeskNumber <= Desks.Count - 1)
            {
                Desks[DeskNumber - 1] = inkCanvas1.Strokes.Clone();
                inkCanvas1.Strokes = Desks[DeskNumber].Clone();
                CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
            }
            else
            {
                Desks[DeskNumber - 1] = inkCanvas1.Strokes.Clone();
                inkCanvas1.Strokes.Clear();
                Desks.Add(inkCanvas1.Strokes.Clone());
                inkCanvas1.Strokes = Desks[DeskNumber].Clone();
                CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
            }
        }
        // Меняем доску на предыдущую
        private void previousDesk(object sender, RoutedEventArgs e)
        {
            if (DeskNumber > 0)
            {
                Desks[DeskNumber] = inkCanvas1.Strokes.Clone();
                DeskNumber--;
                inkCanvas1.Strokes = Desks[DeskNumber].Clone();
                CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
            }
            else
            {
                MessageBox.Show("Last desk");
            }
        }
        // Добавляем новую доску
        private void addDesk(object sender, RoutedEventArgs e)
        {
            Desks.Add(inkCanvas1.Strokes.Clone());
        }

        private void SaveDesks(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "udf files|*.udf";
            saveFileDialog.ShowDialog();
            string[] desks = new string[Desks.Count];
            StrokeCollectionConverter converter = new StrokeCollectionConverter();
            for (int i = 0; i < Desks.Count; i++)
            {
                desks[i] = converter.ConvertToString(Desks[i]);
            }
            string json = JsonConvert.SerializeObject(desks);
            File.WriteAllText(saveFileDialog.FileName, json);
        }
        private void LoadDesks(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "udf files|*.udf";
            openFileDialog.ShowDialog();

            StrokeCollectionConverter converter = new StrokeCollectionConverter();
            Desks.Clear();
            string readText = File.ReadAllText(openFileDialog.FileName);
            string[] desks = JsonConvert.DeserializeObject<string[]>(readText);
            foreach (var i in desks)
            {
                Desks.Add((StrokeCollection)converter.ConvertFromString(i));
            }
            DeskNumber = 0;
            inkCanvas1.Strokes = Desks[DeskNumber].Clone();
        }
    }

    // Класс для определения цветов
    public class ColorARGB { public byte A, R, G, B; }

}
