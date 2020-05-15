using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace Perceptron
{
    public static class PerceptronCore
    {
        public static void RunPerceptron(AllSettings settings, DataGrid resultsGrid, TextBox resultsTextBox)
        {
            List<Result> results = new List<Result>();
            resultsTextBox.Text = string.Empty;

            resultsTextBox.Text += "=== Empezando la evaluación de aptitud para \"Fútbol Americano\" ===\n";
            bool[] americanResults = ExecutePerceptron(settings.ParameterSettings.MaxIterations, settings.ParameterSettings.AcceptableError, settings.ParameterSettings.Alpha,
                settings.ParameterSettings.Bias, settings.ParameterSettings.AmericanFootballSettings, settings.TestData, res => res.AptoParaAmericano, settings.InputData, resultsTextBox);

            resultsTextBox.Text += "=== Empezando la evaluación de aptitud para \"Gimnasia\" ===\n";
            bool[] gymnasticsResults = ExecutePerceptron(settings.ParameterSettings.MaxIterations, settings.ParameterSettings.AcceptableError, settings.ParameterSettings.Alpha,
                settings.ParameterSettings.Bias, settings.ParameterSettings.GymnasticsSettings, settings.TestData, res => res.AptoParaGimnasia, settings.InputData, resultsTextBox);

            int size = americanResults.Length;
            for (int i = 0; i < size; i++)
            {
                results.Add(new Result()
                {
                    Edad = settings.InputData[i].Edad,
                    Estatura = settings.InputData[i].Estatura,
                    Peso = settings.InputData[i].Peso,
                    AptoParaAmericano = americanResults[i],
                    AptoParaGimnasia = gymnasticsResults[i]
                });
            }

            resultsGrid.ItemsSource = results;
        }

        public static bool[] ExecutePerceptron(int maxIterations, float acceptableError, float alpha, float initialBias,
            SportSettings settings, List<Result> trainingCases, Func<Result, bool> expectedResultGetter, List<InputData> toEvaluate, TextBox resultsTextBox)
        {
            float[] weights = GenerateWeights(settings, trainingCases, expectedResultGetter, acceptableError, alpha, maxIterations, initialBias, out float bias);
            float weightsError = GetError(settings, trainingCases, expectedResultGetter, weights, bias);
            int toEvaluateSize = toEvaluate.Count;
            bool[] results = new bool[toEvaluateSize];

            for (int i = 0; i < toEvaluateSize; i++)
            {
                InputData next = toEvaluate[i];
                results[i] = GetResult(GetCriteriaResults(settings, next), weights, bias);
            }

            resultsTextBox.Text += $"La influencia de cada una de las características físicas para determinar la aptitud fue la siguiente:\nEdad: {weights[0]*100}% | Estatura: {weights[1]*100}% | IMC: {weights[2]*100}%\n";
            resultsTextBox.Text += $"El error final para estos pesos fue de {weightsError * 100}%.\n\n";

            return results;
        }

        private static float[] GenerateWeights(SportSettings settings, List<Result> trainingCases, Func<Result, bool> expectedResultGetter,
            float acceptableError, float alpha, int maxIterations, float initialBias, out float bias)
        {
            float[] weights = new float[3]; // Age, Height, IMC
            int testCount = trainingCases.Count;
            float currentBias = initialBias;
            float totalError = float.MaxValue;

            int elapsedIterations = 0;
            // To avoid an infinite loop, either a loop count hard-cap or acceptable error threshold will be reached.
            while (elapsedIterations < maxIterations && totalError > acceptableError)
            {
                for (int i = 0; i < testCount; ++i)
                {
                    Result testCase = trainingCases[i];
                    bool expectedResult = expectedResultGetter(testCase);
                    bool output = GetResult(GetCriteriaResults(settings, testCase), weights, currentBias);
                    int delta = (expectedResult ? 1 : 0) - (output ? 1 : 0);

                    weights = UpdateWeights(weights, alpha, delta, settings, testCase);

                    currentBias += alpha * delta;
                }

                totalError = GetError(settings, trainingCases, expectedResultGetter, weights, currentBias);
                elapsedIterations++;
            }
            bias = currentBias;
            return weights;
        }

        public static float GetError(SportSettings settings, List<Result> trainingCases, Func<Result, bool> expectedResultGetter, float[] currentWeights, float bias)
        {
            float errorSum = 0;
            int size = trainingCases.Count;
            for (int i = 0; i < size; ++i)
            {
                Result testCase = trainingCases[i];
                bool expectedResult = expectedResultGetter(testCase);
                bool output = GetResult(GetCriteriaResults(settings, testCase), currentWeights, bias);
                errorSum += Math.Abs((expectedResult ? 1 : 0) - (output ? 1 : 0));
            }
            return errorSum / ((float)size);
        }

        public static float[] UpdateWeights(float[] currentWeights, float alpha, float delta, SportSettings settings, IEvaluatable toEvaluate)
        {
            bool[] criteriaResults = GetCriteriaResults(settings, toEvaluate);
            float[] result = new float[3]; // Age, Height, IMC
            int size = result.Length;
            for (int i = 0; i < size; i++)
            {
                result[i] = currentWeights[i] + (alpha * delta * (criteriaResults[i] ? 1 : 0));
            }
            return result;
        }

        /// <summary>
        /// Obtains an array that describes what sets of criteria a data set has "cleared", and which they haven't.
        /// </summary>
        public static bool[] GetCriteriaResults(SportSettings settings, IEvaluatable toEvaluate)
        {
            return new bool[]
            {
                (settings.MinAge <= toEvaluate.Edad && toEvaluate.Edad <= settings.MaxAge),
                (settings.MinHeight <= toEvaluate.Estatura && toEvaluate.Estatura <= settings.MaxHeight),
                (settings.MinIMC <= toEvaluate.IMC && toEvaluate.IMC <= settings.MaxIMC)
            };
        }

        /// <summary>
        /// Obtains a true/false to determine if an array of evaluated criteria, with the provided criteria weights equate to a member of the evaluated family.
        /// </summary>
        public static bool GetResult(bool[] criteriaResult, float[] criteriaWeights, float bias)
        {
            float res = 0;
            int size = criteriaResult.Length;
            for (int i = 0; i < size; ++i) { res += ((criteriaResult[i] ? 1 : 0) * criteriaWeights[i]); }
            return res + bias > 0.5;
        }
    }
}