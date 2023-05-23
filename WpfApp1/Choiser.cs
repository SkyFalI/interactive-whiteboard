using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WpfApp1 {
    internal abstract class Choiser {
        internal static void ChoicePen(ref InkCanvas inkCanvas1) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            inkCanvas1.UseCustomCursor = false;
        }
        // Выбираем "Выбрать"
        internal static void ChoiceSelect(ref InkCanvas inkCanvas1) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Select;
            inkCanvas1.UseCustomCursor = false;
        }
        // Временный объект
        internal static void ChoiceTemp(ref InkCanvas inkCanvas1) {
            inkCanvas1.EditingMode = InkCanvasEditingMode.GestureOnly;
            inkCanvas1.UseCustomCursor = false;
        }
    }
}
