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
using iTextSharp.text.pdf;

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

        public MainWindow() {
            InitializeComponent();

            inkCanvas1.Cursor = Cursors.Cross;

            mcolor = new ColorARGB();
            clr = Color.FromArgb(255, 0, 0, 0);
            inkCanvas1.DefaultDrawingAttributes.Color = clr;

            Desks.Add(inkCanvas1.Strokes.Clone());
        }


        // Отслеживания нажатия на клавиатуру
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            //Изменение масштаба пера
            InkWidth.Value += Convert.ToInt32(e.Key == Key.OemPlus || e.Key == Key.Add);
            InkWidth.Value -= Convert.ToInt32(e.Key == Key.OemMinus || e.Key == Key.Subtract);
            // Быстрые действия
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                inkCanvas1.EditingMode = InkCanvasEditingMode.None;
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
            if (e.Key == Key.B) {
                if ((e.KeyboardDevice.Modifiers & combCtrSh) == combCtrSh)
                    MessageBox.Show("Ctrl+Shift+B");
            }

        }
        // Отменить дейсвтие
        public void Undo(object sender, RoutedEventArgs e) {
            EraserButton.IsChecked = false;
            EraserStroke.IsChecked = false;
            if (inkCanvas1.Strokes.Count > 0) inkCanvas1.Strokes.RemoveAt(inkCanvas1.Strokes.Count - 1);
        }
        // Кнопка вернуть
        public void Redo(object sender, RoutedEventArgs e) {
            EraserButton.IsChecked = false;
            EraserStroke.IsChecked = false;
            try {
                inkCanvas1.Strokes.Add(Desks[DeskNumber][inkCanvas1.Strokes.Count]);
            } catch { MessageBox.Show("Нечего возвращать"); }
        }

        // Очистка доски
        private void ClearCanvas(object sender, RoutedEventArgs e) { inkCanvas1.Strokes.Clear(); }

        // Закрытие приложения
        private void CloseApp(object sender, RoutedEventArgs e) { this.Close(); }

        // Сохраняем свое творчество в pdf используя iTextSharp из NuGet пакетов
        private void SaveCanvas(object sender, RoutedEventArgs e) {

            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "pdf files|*.pdf";
            SFD.ShowDialog();

            // https://stackoverflow.com/questions/21411878/saving-a-canvas-to-png-c-sharp-wpf
            if (SFD.FileName == "") return;
            var stream = new FileStream(SFD.FileName, FileMode.Append, FileAccess.Write, FileShare.None);

            iTextSharp.text.Document document = new iTextSharp.text.Document();
            document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            document.SetMargins(10, 10, 10, 10);

            PdfWriter.GetInstance(document, stream);
            document.Open();

            Rect bounds = VisualTreeHelper.GetDescendantBounds(inkCanvas1);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen()) {
                VisualBrush vb = new VisualBrush(inkCanvas1);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            MemoryStream fs = new MemoryStream();
            JpegBitmapEncoder encoder1 = new JpegBitmapEncoder();
            encoder1.Frames.Add(BitmapFrame.Create(rtb));
            encoder1.Save(fs);
            byte[] tArr = fs.ToArray();

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(tArr);

            image.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            image.ScaleAbsolute(120f, 155.25f);

            document.Add(image);
            document.NewPage();
            fs.Close();
            rtb.Clear();

            document.Close();

        }

        // Меняем толщину кисти
        private void NumericLimit(object sender, RoutedPropertyChangedEventArgs<object> e) {
            try {
                inkCanvas1.DefaultDrawingAttributes.Width = (int)InkWidth.Value;
                inkCanvas1.DefaultDrawingAttributes.Height = (int)InkWidth.Value;
            } catch { InkWidth.Value = 1; }

        }
        // Выбираем ластик
        private void ChoiceEraser(object sender, RoutedEventArgs e) {

            /*
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            inkCanvas1.DefaultDrawingAttributes.Color = Color.FromArgb(255, 255, 255, 255);
            inkCanvas1.UseCustomCursor = true;
            */
            if (EraserButton.IsChecked == true)
                inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint;
            else
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }
        private void ChoiceEraserBytStroke(object sender, RoutedEventArgs e) {
            if (EraserStroke.IsChecked == true)
                inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByStroke;
            else
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }

        // Выбираем кисть
        private void ChoicePen(object sender, RoutedEventArgs e) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            inkCanvas1.DefaultDrawingAttributes.Color = clr;
            inkCanvas1.UseCustomCursor = false;
        }
        // Выбираем "Выбрать"
        private void ChoiceSelect(object sender, RoutedEventArgs e) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Select;
            inkCanvas1.UseCustomCursor = false;
        }
        // Временный объект
        private void ChoiceTemp(object sender, RoutedEventArgs e) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.GestureOnly;
            inkCanvas1.DefaultDrawingAttributes.Color = clr;
            inkCanvas1.UseCustomCursor = false;
        }

        // Отслеживаем координаты мыши
        private void MouseMove1(object sender, MouseEventArgs e) {
            textBlock1.Text = "X = " + e.GetPosition(null).X.ToString() + "\nY = " + e.GetPosition(null).Y.ToString();

            /*          Отслеживавние нажатия на левую кнопку мыши    
                        if (Mouse.LeftButton == MouseButtonState.Pressed)
                        {      
                        }
            */

        }
        // Раскрываем colorCanvas для выбора цвета
        private void ColorPicker(object sender, RoutedEventArgs e) { colorPicker1.Visibility = Visibility; }
        // При потери фокуса у ColorPicker смена цвета кисти
        private void PickerHide(object sender, MouseEventArgs e) {

            colorPicker1.Visibility = Visibility.Hidden;

            mcolor.A = colorPicker1.A;
            mcolor.R = colorPicker1.R;
            mcolor.G = colorPicker1.G;
            mcolor.B = colorPicker1.B;

            clr = Color.FromArgb(mcolor.A, mcolor.R, mcolor.G, mcolor.B);

            inkCanvas1.DefaultDrawingAttributes.Color = clr;
        }
        // Клонируем inkCanvas1 в temp для UnDo/ReDo
        private void MouseLeftButtonUp1(object sender, MouseButtonEventArgs e) {
            if (EraserStroke.IsChecked == false & EraserButton.IsChecked == false) {
                Desks[DeskNumber] = inkCanvas1.Strokes.Clone();
                MessageBox.Show("Save!");
            }
        }
        // Меняем доску на следующую
        private void nextDesk(object sender, RoutedEventArgs e) {
            DeskNumber++;
            if (DeskNumber <= Desks.Count - 1) {
                Desks[DeskNumber - 1] = inkCanvas1.Strokes.Clone();
                inkCanvas1.Strokes = Desks[DeskNumber].Clone();
                CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
            }
            else {
                Desks[DeskNumber - 1] = inkCanvas1.Strokes.Clone();
                inkCanvas1.Strokes.Clear();
                Desks.Add(inkCanvas1.Strokes.Clone());
                inkCanvas1.Strokes = Desks[DeskNumber].Clone();
                CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
            }
        }
        // Меняем доску на предыдущую
        private void previousDesk(object sender, RoutedEventArgs e) {
            if (DeskNumber > 0) {
                Desks[DeskNumber] = inkCanvas1.Strokes.Clone();
                DeskNumber--;
                inkCanvas1.Strokes = Desks[DeskNumber].Clone();
                CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
            }
            else {
                MessageBox.Show("Last desk");
            }
        }
        // Добавляем новую доску
        private void addDesk(object sender, RoutedEventArgs e) {
            Desks.Add(inkCanvas1.Strokes.Clone());
        }

        private void SaveDesks(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "udf files|*.udf";
            saveFileDialog.ShowDialog();
            string[] desks = new string[Desks.Count];
            StrokeCollectionConverter converter = new StrokeCollectionConverter();

            if (saveFileDialog.FileName != "") {
                for (int i = 0; i < Desks.Count; i++) {
                    desks[i] = converter.ConvertToString(Desks[i]);
                }
                string json = JsonConvert.SerializeObject(desks);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }
        private void LoadDesks(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "udf files|*.udf";
            openFileDialog.ShowDialog();

            StrokeCollectionConverter converter = new StrokeCollectionConverter();
            Desks.Clear();
            string readText = File.ReadAllText(openFileDialog.FileName);
            string[] desks = JsonConvert.DeserializeObject<string[]>(readText);
            foreach (var i in desks) {
                Desks.Add((StrokeCollection)converter.ConvertFromString(i));
            }
            DeskNumber = 0;
            inkCanvas1.Strokes = Desks[DeskNumber].Clone();
        }

        private void ChoiceHand(object sender, RoutedEventArgs e) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.None;
        }

        private void ReturnINK(object sender, KeyEventArgs e) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void onOffFXAA(object sender, RoutedEventArgs e) {

            if (FXAA.IsChecked == true)
                inkCanvas1.DefaultDrawingAttributes.FitToCurve = true;            
            else 
                inkCanvas1.DefaultDrawingAttributes.FitToCurve = false;
            
        }

        private void onOffHighlither(object sender, RoutedEventArgs e) {
            if (Highlither.IsChecked == true) 
                inkCanvas1.DefaultDrawingAttributes.IsHighlighter = true;            
            else
                inkCanvas1.DefaultDrawingAttributes.IsHighlighter = false;
        }

    }

    // Класс для определения цветов
    public class ColorARGB { public byte A, R, G, B; }

}
