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
    }
}