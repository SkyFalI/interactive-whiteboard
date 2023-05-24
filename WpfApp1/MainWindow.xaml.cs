using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Threading;

// https://github.com/xceedsoftware/wpftoolkit EXTENDED WPF TOOLS KIT

namespace WpfApp1 {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private ColorARGB mcolor { get; set; }
        private Color clr { get; set; }

        internal DeskStore _deskStore;
        internal InkStore _inkStore;

        public MainWindow() {

            InitializeComponent();

            mcolor = new ColorARGB();

            _inkStore = new InkStore(inkCanvas1);
            _deskStore = new DeskStore();

            _deskStore.Desks.Add(inkCanvas1.Strokes.Clone());

        }


        // Отслеживания нажатия на клавиатуру
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            //Изменение масштаба пера
            InkWidth.Value += Convert.ToInt32(e.Key == Key.OemPlus || e.Key == Key.Add);
            InkWidth.Value -= Convert.ToInt32(e.Key == Key.OemMinus || e.Key == Key.Subtract);
            // Быстрые действия
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                _inkStore.inkCanvas.EditingMode = InkCanvasEditingMode.None;
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
            if (_inkStore.inkCanvas.Strokes.Count > 0) _inkStore.inkCanvas.Strokes.RemoveAt(_inkStore.inkCanvas.Strokes.Count - 1);
        }
        // Кнопка вернуть
        public void Redo(object sender, RoutedEventArgs e) {
            EraserButton.IsChecked = false;
            EraserStroke.IsChecked = false;
            try {
                _inkStore.inkCanvas.Strokes.Add(_deskStore.Desks[_deskStore.deskNumber][_inkStore.inkCanvas.Strokes.Count]);
            } catch { MessageBox.Show("Нечего возвращать"); }
        }

        // Очистка доски
        private void ClearCanvas(object sender, RoutedEventArgs e) { _inkStore.inkCanvas.Strokes.Clear(); }

        // Сохраняем свое творчество в pdf используя iTextSharp из NuGet пакетов
        private void SaveCanvas(object sender, RoutedEventArgs e) {

            _inkStore.SaveAsPDF(_deskStore.Desks);
        }

        // Меняем толщину кисти
        private void NumericLimit(object sender, RoutedPropertyChangedEventArgs<object> e) {
            try {
                inkCanvas1.DefaultDrawingAttributes.Width = (int)InkWidth.Value;
                inkCanvas1.DefaultDrawingAttributes.Height = (int)InkWidth.Value;
                inkCanvas1.EraserShape = new RectangleStylusShape((int)InkWidth.Value, (int)InkWidth.Value);
            } catch { InkWidth.Value = 1; }

        }
        // Выбираем ластик
        private void ChoiceEraser(object sender, RoutedEventArgs e) {
            Choiser.ChoiceEraser(ref inkCanvas1, ref EraserButton);
        }
        private void ChoiceEraserBytStroke(object sender, RoutedEventArgs e) {
            Choiser.ChoiceEraserBytStroke(ref inkCanvas1, ref EraserStroke);
        }

        // Выбираем кисть
        private void ChoicePen(object sender, RoutedEventArgs e) {
            Choiser.ChoicePen(ref inkCanvas1);
        }
        // Выбираем "Выбрать"
        private void ChoiceSelect(object sender, RoutedEventArgs e) {
            Choiser.ChoiceSelect(ref inkCanvas1);
        }
        // Временный объект
        private void ChoiceTemp(object sender, RoutedEventArgs e) {
            Choiser.ChoiceTemp(ref inkCanvas1);
        }

        // Отслеживаем координаты мыши
        private void MouseMove1(object sender, MouseEventArgs e) {
            textBlock1.Text = "X = " + Convert.ToInt32(e.GetPosition(_inkStore.inkCanvas).X).ToString() + "\nY = " + Convert.ToInt32(e.GetPosition(_inkStore.inkCanvas).Y).ToString();
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                if (e.GetPosition(_inkStore.inkCanvas).X > _inkStore.inkCanvas.Width - 15) _inkStore.inkCanvas.Width += 20;
                if (e.GetPosition(_inkStore.inkCanvas).Y > _inkStore.inkCanvas.Height - 15) _inkStore.inkCanvas.Height += 20;

            }

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
                _deskStore.Desks[_deskStore.deskNumber] = inkCanvas1.Strokes.Clone();
            }
        }
        // Меняем доску на следующую
        private void nextDesk(object sender, RoutedEventArgs e) {
            _deskStore.nextDesk(ref inkCanvas1, CurrentDesk);
        }
        // Меняем доску на предыдущую
        private void previousDesk(object sender, RoutedEventArgs e) {
            _deskStore.previousDesk(ref inkCanvas1, CurrentDesk);
        }
        // Добавляем новую доску
        private void addDesk(object sender, RoutedEventArgs e) {
            _deskStore.addDesk(ref inkCanvas1);
        }

        private void SaveDesks(object sender, RoutedEventArgs e) {
            _deskStore.SaveDesks(ref inkCanvas1);
        }
        private void LoadDesks(object sender, RoutedEventArgs e) {
            _deskStore.LoadDesks(ref inkCanvas1, CurrentDesk);
        }

        private void ChoiceHand(object sender, RoutedEventArgs e) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.None;
        }

        private void ReturnINK(object sender, KeyEventArgs e) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void onOffFXAA(object sender, RoutedEventArgs e) {
            Choiser.onOffFXAA(ref inkCanvas1, ref FXAA);         
        }

        private void onOffHighlither(object sender, RoutedEventArgs e) {
            Choiser.onOffHighlither(ref inkCanvas1, ref Highlither);
        }

    }

    // Класс для определения цветов
    public class ColorARGB { public byte A, R, G, B; }

}
