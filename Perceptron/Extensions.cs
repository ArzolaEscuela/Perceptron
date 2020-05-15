using System.Windows.Controls;

namespace Perceptron
{
    public static class Extensions
    {
        public static int AsInt(this TextBox tb)
        {
            if (int.TryParse(tb.Text, out int result)) { return result; }
            return -1;
        }

        public static float AsFloat(this TextBox tb)
        {
            if (float.TryParse(tb.Text, out float result)) { return result; }
            return -1.0f;
        }

        public static double AsDouble(this TextBox tb)
        {
            if (double.TryParse(tb.Text, out double result)) { return result; }
            return -1.0f;
        }

        public static float AsFloat(this Slider slider) => (float)slider.Value;

        public static string ContentsAsString<T>(this T[] array)
        {
            if (array.Length == 0) { return "[]"; }

            string result = $"[{array[0].ToString()}";
            int size = array.Length;
            for (int i = 1; i < size; i++)
            {
                result = $"{result}, {array[i].ToString()}";
            }
            return $"{result}]";
        }
    }
}