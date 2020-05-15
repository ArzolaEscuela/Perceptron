using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace Perceptron
{
    public class SportSettings
    {
        public int MinAge { get; set; } = 0;
        public int MaxAge { get; set; } = 0;
        public float AgeWeight { get; set; } = 0;

        public float MinHeight { get; set; } = 0;
        public float MaxHeight { get; set; } = 0;
        public float HeightWeight { get; set; } = 0;

        public float MinIMC { get; set; } = 0;
        public float MaxIMC { get; set; } = 0;
        public float IMCWeight { get; set; } = 0;
    }

    public class Settings
    {
        public float Alpha { get; set; } = 0.0f;
        public float Bias { get; set; } = 0.0f;
        public float AcceptableError { get; set; } = 0.0f;
        public int MaxIterations { get; set; } = 1000;

        public SportSettings GymnasticsSettings { get; set; } = new SportSettings();
        public SportSettings AmericanFootballSettings { get; set; } = new SportSettings();
    }

    public class AllSettings
    {
        public Settings ParameterSettings { get; set; } = new Settings();
        public List<Result> TestData { get; set; } = new List<Result>();
        public List<InputData> InputData { get; set; } = new List<InputData>();
    }

    class SaveManager
    {
        //------------------------------------------------------------------------------------//
        /*--------------------------------- PROPERTIES ---------------------------------------*/
        //------------------------------------------------------------------------------------//

        private static string FullSettingsFilePath => $"{Environment.CurrentDirectory}/Settings.json";

        private static AllSettings CurrentSettings => new AllSettings()
        {
            TestData = MainWindow.testData,
            InputData = MainWindow.inputData,
            ParameterSettings = MainWindow.CurrentSettings
        };

        //------------------------------------------------------------------------------------//
        /*---------------------------------- METHODS -----------------------------------------*/
        //------------------------------------------------------------------------------------//

        public static AllSettings LoadSettings()
        {
            if (File.Exists(FullSettingsFilePath))
            {
                string json = File.ReadAllText(FullSettingsFilePath);
                return JsonConvert.DeserializeObject<AllSettings>(json);
            }
            return new AllSettings();
        }

        public static void SaveChanges()
        {
            string json = JsonConvert.SerializeObject(CurrentSettings, Formatting.Indented);
            File.WriteAllText(FullSettingsFilePath, json);
        }
    }
}