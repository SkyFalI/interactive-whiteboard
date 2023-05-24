using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace WpfApp1 {
    internal class InkStore {

        public InkCanvas inkCanvas;

        public InkStore(InkCanvas inkCanvas) {
            this.inkCanvas = inkCanvas;
        }


        internal void SaveAsPDF(List<StrokeCollection> Desks) {

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

            StrokeCollection temp = inkCanvas.Strokes;

            for (int i = 0; i < Desks.Count; i++) {
                inkCanvas.Strokes = Desks[i];
                Rect bounds = VisualTreeHelper.GetDescendantBounds(inkCanvas);
                double dpi = 96d;

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

                DrawingVisual dv = new DrawingVisual();

                using (DrawingContext dc = dv.RenderOpen()) {
                    VisualBrush vb = new VisualBrush(inkCanvas);
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
                image.ScaleToFit(document.PageSize.Width - 10, document.PageSize.Height - 10);

                document.Add(image);
                document.NewPage();
                fs.Close();
                rtb.Clear();
            }

            inkCanvas.Strokes = temp;

            document.Close();

        }
    }
}
