using Microsoft.Win32;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows;
using System.Collections.Generic;
using System.IO;

namespace WpfApp1 {
    internal class DeskStore {

        public List<StrokeCollection> Desks { get; private set; }
        public int deskNumber { get; private set; }

        public DeskStore() {
            Desks = new List<StrokeCollection>();
            deskNumber = 0;
        }

        public void addDesk(ref InkCanvas inkCanvas1) {
            Desks.Add(inkCanvas1.Strokes.Clone());
        }

        public void SaveDesks(ref InkCanvas inkCanvas1) {
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
        public void LoadDesks(ref InkCanvas inkCanvas1, TextBlock CurrentDesk) {
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
            this.deskNumber = 0;
            inkCanvas1.Strokes = Desks[this.deskNumber].Clone();
            CurrentDesk.Text = $"{(this.deskNumber + 1).ToString()} / {Desks.Count}";
        }

        public void nextDesk(ref InkCanvas inkCanvas1, TextBlock CurrentDesk) {
            deskNumber++;
            if (deskNumber <= Desks.Count - 1) {
                Desks[deskNumber - 1] = inkCanvas1.Strokes.Clone();
                inkCanvas1.Strokes = Desks[deskNumber].Clone();
                CurrentDesk.Text = $"{(deskNumber + 1).ToString()} / {Desks.Count}";
            }
            else {

                Desks[deskNumber - 1] = inkCanvas1.Strokes.Clone();
                inkCanvas1.Strokes.Clear();
                Desks.Add(inkCanvas1.Strokes.Clone());
                inkCanvas1.Strokes = Desks[deskNumber].Clone();
                CurrentDesk.Text = $"{(deskNumber + 1).ToString()} / {Desks.Count}";
            }
        }
        // Меняем доску на предыдущую
        public void previousDesk(ref InkCanvas inkCanvas1, TextBlock CurrentDesk) {
            if (deskNumber > 0) {
                Desks[deskNumber] = inkCanvas1.Strokes.Clone();
                deskNumber--;
                inkCanvas1.Strokes = Desks[deskNumber].Clone();
                CurrentDesk.Text = $"{(deskNumber + 1).ToString()} / {Desks.Count}";
            }
            else {
                MessageBox.Show("Last desk");
            }
        }
    }
}
