using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Perceptron
{
    public interface IEvaluatable
    {
        int Edad { get; set; }
        float Estatura { get; set; }
        float IMC { get; }
    }

    public class Result : IEvaluatable
    {
        private int _edad;
        private float _estatura;
        private float _peso;
        private bool _aptoParaAmericano;
        private bool _aptoParaGimnasia;

        public int Edad
        {
            get => _edad;
            set { _edad = value; MainWindow.OnCellChanged(); }
        }

        public float Estatura
        {
            get => _estatura;
            set { _estatura = value; MainWindow.OnCellChanged(); }
        }

        public float Peso
        {
            get => _peso;
            set { _peso = value; MainWindow.OnCellChanged(); }
        }

        [JsonIgnore]
        public float IMC => Math.Abs(Estatura) < 0.01 ? 0 : Peso / (Estatura * Estatura);

        public bool AptoParaAmericano
        {
            get => _aptoParaAmericano;
            set { _aptoParaAmericano = value; MainWindow.OnCellChanged(); }
        }

        public bool AptoParaGimnasia
        {
            get => _aptoParaGimnasia;
            set { _aptoParaGimnasia = value; MainWindow.OnCellChanged(); }
        }
    }

    public class InputData : IEvaluatable
    {
        private int _edad;
        private float _estatura;
        private float _peso;

        public int Edad
        {
            get => _edad;
            set { _edad = value; MainWindow.OnCellChanged(); }
        }

        public float Estatura
        {
            get => _estatura;
            set { _estatura = value; MainWindow.OnCellChanged(); }
        }

        public float Peso
        {
            get => _peso;
            set { _peso = value; MainWindow.OnCellChanged(); }
        }

        [JsonIgnore]
        public float IMC => Math.Abs(Estatura) < 0.01 ? 0 : Peso / (Estatura * Estatura);
    }

    public partial class MainWindow : Window
    {
        //------------------------------------------------------------------------------------//
        /*----------------------------------- FIELDS -----------------------------------------*/
        //------------------------------------------------------------------------------------//

        private static MainWindow _instance;
        public static List<Result> testData = new List<Result>();
        public static List<InputData> inputData = new List<InputData>();
        private static List<Result> _results = new List<Result>();

        //------------------------------------------------------------------------------------//
        /*--------------------------------- PROPERTIES ---------------------------------------*/
        //------------------------------------------------------------------------------------//

        public static Settings CurrentSettings => _instance._CurrentSettings;

        private Settings _CurrentSettings => new Settings()
        {
            AmericanFootballSettings = new SportSettings()
            {
                MinAge = American_Min_Age.AsInt(),
                MaxAge = American_Max_Age.AsInt(),
                AgeWeight = American_Age_Weight.AsFloat(),

                MinHeight = American_Height_Min.AsFloat(),
                MaxHeight = American_Height_Max.AsFloat(),
                HeightWeight = American_Height_Weight.AsFloat(),

                MinIMC = American_IMC_Min.AsFloat(),
                MaxIMC = American_IMC_Max.AsFloat(),
                IMCWeight = American_IMC_Weight.AsFloat(),
            },
            GymnasticsSettings = new SportSettings()
            {
                MinAge = Gymnastics_Min_Age.AsInt(),
                MaxAge = Gymnastics_Max_Age.AsInt(),
                AgeWeight = Gymnastics_Age_Weight.AsFloat(),

                MinHeight = Gymnastics_Height_Min.AsFloat(),
                MaxHeight = Gymnastics_Height_Max.AsFloat(),
                HeightWeight = Gymnastics_Height_Weight.AsFloat(),

                MinIMC = Gymnastics_IMC_Min.AsFloat(),
                MaxIMC = Gymnastics_IMC_Max.AsFloat(),
                IMCWeight = Gymnastics_IMC_Weight.AsFloat(),
            },
            MaxIterations = Max_Iterations.AsInt(),
            Alpha = Alpha.AsFloat(),
            Bias = Bias.AsFloat(),
            AcceptableError = Acceptable_Error.AsFloat()
        };

        //------------------------------------------------------------------------------------//
        /*---------------------------------- METHODS -----------------------------------------*/
        //------------------------------------------------------------------------------------//

        public MainWindow()
        {
            InitializeComponent();
            _instance = this;

            InitializeSettings();

            Training_DataGrid.ItemsSource = testData;
            Tests_DataGrid.ItemsSource = inputData;
            Results_DataGrid.ItemsSource = _results;

            CheckForErrors();
        }

        private void CheckForErrors()
        {
            string errors = string.Empty;

            double americanTotal = American_Age_Weight.AsFloat() + American_Height_Weight.AsFloat() + American_IMC_Weight.AsFloat();
            if (Math.Abs(americanTotal - 100) > 0.01)
            {
                errors += $"La suma de los % de aptitud para los criterios de Futbol Americano deben dar 100%, actualmente da: {americanTotal}%.\n";
            }
            double gymnasticsTotal = Gymnastics_Age_Weight.AsFloat() + Gymnastics_Height_Weight.AsFloat() + Gymnastics_IMC_Weight.AsFloat();
            if (Math.Abs(gymnasticsTotal - 100) > 0.01)
            {
                errors += $"La suma de los % de aptitud para los criterios de Gimnasia deben dar 100%, actualmente da: {gymnasticsTotal}%.\n";
            }

            Errors_Textbox.Text = errors;
        }

        #region Settings

        private void BindSliders(Slider slider, TextBox percentageBox)
        {
            slider.ValueChanged += (sender, args) =>
            {
                string newValue = slider.AsFloat().ToString(CultureInfo.InvariantCulture);
                if (percentageBox.Text.Equals(newValue)) { return; }
                percentageBox.Text = newValue;
            };
            percentageBox.TextChanged += (sender, args) =>
            {
                double newValue = percentageBox.AsDouble();
                if (Math.Abs(slider.Value - newValue) < 0.01) { return; }
                slider.Value = newValue;
            };
        }

        private static async Task LateRefresh()
        {
            await Task.Delay(1);

            _instance.Training_DataGrid.Items.Refresh();
            _instance.Tests_DataGrid.Items.Refresh();
            _instance.CheckForErrors();
        }
        public static void OnCellChanged()
        {
            LateRefresh();
            SaveManager.SaveChanges();
        }
        private void OnValueChanged(object sender, TextChangedEventArgs e) { SaveManager.SaveChanges(); CheckForErrors(); }

        private void InitializeSettings()
        {
            #region Bind On Value Changed Events

            American_Min_Age.TextChanged += OnValueChanged;
            American_Max_Age.TextChanged += OnValueChanged;
            American_Age_Weight.TextChanged += OnValueChanged;

            American_Height_Min.TextChanged += OnValueChanged;
            American_Height_Max.TextChanged += OnValueChanged;
            American_Height_Weight.TextChanged += OnValueChanged;

            American_IMC_Min.TextChanged += OnValueChanged;
            American_IMC_Max.TextChanged += OnValueChanged;
            American_IMC_Weight.TextChanged += OnValueChanged;

            Gymnastics_Min_Age.TextChanged += OnValueChanged;
            Gymnastics_Max_Age.TextChanged += OnValueChanged;
            Gymnastics_Age_Weight.TextChanged += OnValueChanged;

            Gymnastics_Height_Min.TextChanged += OnValueChanged;
            Gymnastics_Height_Max.TextChanged += OnValueChanged;
            Gymnastics_Height_Weight.TextChanged += OnValueChanged;

            Gymnastics_IMC_Min.TextChanged += OnValueChanged;
            Gymnastics_IMC_Max.TextChanged += OnValueChanged;
            Gymnastics_IMC_Weight.TextChanged += OnValueChanged;

            Acceptable_Error.TextChanged += OnValueChanged;
            Max_Iterations.TextChanged += OnValueChanged;
            Alpha.TextChanged += OnValueChanged;
            Bias.TextChanged += OnValueChanged;

            #endregion Bind On Value Changed Events

            #region Bind Sliders

            BindSliders(American_Age_Slider, American_Age_Weight);
            BindSliders(American_Height_Slider, American_Height_Weight);
            BindSliders(American_IMC_Slider, American_IMC_Weight);

            BindSliders(Gymnastics_Age_Slider, Gymnastics_Age_Weight);
            BindSliders(Gymnastics_Height_Slider, Gymnastics_Height_Weight);
            BindSliders(Gymnastics_IMC_Slider, Gymnastics_IMC_Weight);

            #endregion Bind Sliders

            #region Initial Values

            AllSettings settings = SaveManager.LoadSettings();

            American_Min_Age.Text = settings.ParameterSettings.AmericanFootballSettings.MinAge.ToString(CultureInfo.InvariantCulture);
            American_Max_Age.Text = settings.ParameterSettings.AmericanFootballSettings.MaxAge.ToString(CultureInfo.InvariantCulture);
            American_Age_Weight.Text = settings.ParameterSettings.AmericanFootballSettings.AgeWeight.ToString(CultureInfo.InvariantCulture);

            American_Height_Min.Text = settings.ParameterSettings.AmericanFootballSettings.MinHeight.ToString(CultureInfo.InvariantCulture);
            American_Height_Max.Text = settings.ParameterSettings.AmericanFootballSettings.MaxHeight.ToString(CultureInfo.InvariantCulture);
            American_Height_Weight.Text = settings.ParameterSettings.AmericanFootballSettings.HeightWeight.ToString(CultureInfo.InvariantCulture);

            American_IMC_Min.Text = settings.ParameterSettings.AmericanFootballSettings.MinIMC.ToString(CultureInfo.InvariantCulture);
            American_IMC_Max.Text = settings.ParameterSettings.AmericanFootballSettings.MaxIMC.ToString(CultureInfo.InvariantCulture);
            American_IMC_Weight.Text = settings.ParameterSettings.AmericanFootballSettings.IMCWeight.ToString(CultureInfo.InvariantCulture);

            Gymnastics_Min_Age.Text = settings.ParameterSettings.GymnasticsSettings.MinAge.ToString(CultureInfo.InvariantCulture);
            Gymnastics_Max_Age.Text = settings.ParameterSettings.GymnasticsSettings.MaxAge.ToString(CultureInfo.InvariantCulture);
            Gymnastics_Age_Weight.Text = settings.ParameterSettings.GymnasticsSettings.AgeWeight.ToString(CultureInfo.InvariantCulture);

            Gymnastics_Height_Min.Text = settings.ParameterSettings.GymnasticsSettings.MinHeight.ToString(CultureInfo.InvariantCulture);
            Gymnastics_Height_Max.Text = settings.ParameterSettings.GymnasticsSettings.MaxHeight.ToString(CultureInfo.InvariantCulture);
            Gymnastics_Height_Weight.Text = settings.ParameterSettings.GymnasticsSettings.HeightWeight.ToString(CultureInfo.InvariantCulture);

            Gymnastics_IMC_Min.Text = settings.ParameterSettings.GymnasticsSettings.MinIMC.ToString(CultureInfo.InvariantCulture);
            Gymnastics_IMC_Max.Text = settings.ParameterSettings.GymnasticsSettings.MaxIMC.ToString(CultureInfo.InvariantCulture);
            Gymnastics_IMC_Weight.Text = settings.ParameterSettings.GymnasticsSettings.IMCWeight.ToString(CultureInfo.InvariantCulture);

            inputData = settings.InputData;
            testData = settings.TestData;

            Acceptable_Error.Text = settings.ParameterSettings.AcceptableError.ToString(CultureInfo.InvariantCulture);
            Max_Iterations.Text = settings.ParameterSettings.MaxIterations.ToString(CultureInfo.InvariantCulture);
            Alpha.Text = settings.ParameterSettings.Alpha.ToString(CultureInfo.InvariantCulture);
            Bias.Text = settings.ParameterSettings.Bias.ToString(CultureInfo.InvariantCulture);

            SaveManager.SaveChanges();

            #endregion Initial Values
        }

        #endregion Settings

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PerceptronCore.RunPerceptron(SaveManager.LoadSettings(), Results_DataGrid, Results_TextBox);
        }
    }
}
