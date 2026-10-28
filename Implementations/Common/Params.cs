using System.Globalization;
using Parameters;

namespace Implementations.Common;

internal static class Params
{
    public static Parameter<int> Int(string key, int min = int.MinValue) =>
        new(key, new Validator<int>(value => ParseInt(key, value, min)));

    public static Parameter<double> Double(
        string key,
        double min = double.NegativeInfinity,
        double max = double.PositiveInfinity) =>
        new(key, new Validator<double>(value => ParseDouble(key, value, min, max)));

    public static Parameter<double> Probability(string key) => Double(key, 0, 1);

    private static int ParseInt(string key, string value, int min)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ||
            result < min)
        {
            throw new FormatException(
                $"Parameter '{key}' must be an integer greater than or equal to {min}.");
        }

        return result;
    }

    private static double ParseDouble(string key, string value, double min, double max)
    {
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) ||
            !double.IsFinite(result) ||
            result < min ||
            result > max)
        {
            throw new FormatException(
                $"Parameter '{key}' must be a finite number in [{min}, {max}].");
        }

        return result;
    }
}

internal static class ParameterSetExtensions
{
    public static T Read<T>(this ParameterSet values, Parameter<T> parameter, T fallback) =>
        values.Any(pair => pair.Key == parameter.Key) ? values.Get(parameter) : fallback;
}