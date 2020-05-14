using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Perceptron
{
    public class Result
    {
        public int Edad { get; set; }
        public float Estatura { get; set; }
        public float Peso { get; set; }
        public float IMC => Math.Abs(Estatura) < 0.01 ? 0 : Peso / (Estatura * Estatura);
        public bool AptoParaAmericano { get; set; }
        public bool AptoParaGimnasia { get; set; }
    }

    public class InputData
    {
        public int Edad { get; set; }
        public float Estatura { get; set; }
        public float Peso { get; set; }
        public float IMC => Math.Abs(Estatura) < 0.01 ? 0 : Peso / (Estatura * Estatura);
    }
    
    public partial class MainWindow : Window
    {

        private static List<Result> _testData = new List<Result>();
        private static List<InputData> _inputData = new List<InputData>();
        private static List<Result> _results = new List<Result>();
        public MainWindow()
        {
            InitializeComponent();
            Training_DataGrid.ItemsSource = _testData;
            Tests_DataGrid.ItemsSource = _inputData;
            Results_DataGrid.ItemsSource = _results;
        }

        #region Number TextBox Events

        private static Dictionary<TextBox, string> _previousTextBoxContents = new Dictionary<TextBox, string>();
        private static bool IsFloat(string text) { return float.TryParse(text, out float result); }
        private void OnFloatTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            string last = _previousTextBoxContents.ContainsKey(tb) ? _previousTextBoxContents[tb] : string.Empty;

            if (!IsFloat(tb.Text)) { tb.Text = last; return; }

            if (!_previousTextBoxContents.ContainsKey(tb))
            {
                _previousTextBoxContents.Add(tb, tb.Text);
                return;
            }

            _previousTextBoxContents[tb] = tb.Text;
        }
        private static bool IsInt(string text) { return int.TryParse(text, out int result); }
        private void OnIntTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            string last = _previousTextBoxContents.ContainsKey(tb) ? _previousTextBoxContents[tb] : string.Empty;

            if (!IsInt(tb.Text)) { tb.Text = last; return; }

            if (!_previousTextBoxContents.ContainsKey(tb))
            {
                _previousTextBoxContents.Add(tb, tb.Text);
                return;
            }

            _previousTextBoxContents[tb] = tb.Text;
        }

        #endregion Number TextBox Events
    }
}
