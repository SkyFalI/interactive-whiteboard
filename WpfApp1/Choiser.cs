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

        internal static void ChoiceEraser(ref InkCanvas inkCanvas1, ref CheckBox EraserButton) {
            if (EraserButton.IsChecked == true)
                inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint;
            else
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }

        internal static void ChoiceEraserBytStroke(ref InkCanvas inkCanvas1, ref CheckBox EraserStroke) {
            if (EraserStroke.IsChecked == true)
                inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByStroke;
            else
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }

        internal static void onOffFXAA(ref InkCanvas inkCanvas1, ref CheckBox Highlither) {
            if (Highlither.IsChecked == true)
                inkCanvas1.DefaultDrawingAttributes.FitToCurve = true;
            else
                inkCanvas1.DefaultDrawingAttributes.FitToCurve = false;
        }
        internal static void onOffHighlither(ref InkCanvas inkCanvas1, ref CheckBox FXAA) {
            if (FXAA.IsChecked == true)
                inkCanvas1.DefaultDrawingAttributes.IsHighlighter = true;
            else
                inkCanvas1.DefaultDrawingAttributes.IsHighlighter = false;
        }
    }
}
