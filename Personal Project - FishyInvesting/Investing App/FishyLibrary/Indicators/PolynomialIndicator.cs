using Newtonsoft.Json.Linq;

namespace FishyLibrary.Indicators;

public class PolynomialIndicator : IIndicator
{
    private Queue<double> _numbers;
    private readonly Func<double> _getLength;

    public PolynomialIndicator(Func<double> getLength)
    {
        _getLength = getLength;
        _numbers = new();
    }

    public void UpdateIndicator(double newPrice)
    {
        _numbers.Enqueue(newPrice);
        if (_numbers.Count > _getLength())
        {
            _numbers.Dequeue();
        }
    }

    private static List<double> FitPolynomial(List<double> y)
    {
        List<List<double>> matrix = new List<List<double>>();
        for (int i = 0; i < y.Count; i++)
        {
            List<double> row = new();
            for (int j = y.Count - 1; j >= 0; j--)
            {
                row.Add(Math.Pow(i + 1, j));
            }

            row.Add(y[i]);
            matrix.Add(row);
        }

        for (int i = 0; i < y.Count - 1; i++)
        {
            List<double> data = matrix[i];
            for (int j = i + 1; j < y.Count; j++)
            {
                List<double> row = matrix[j];
                double value = row[i] / data[i];
                for (int k = i; k < data.Count; k++)
                {
                    row[k] -= (value * data[k]);
                }

                matrix[j] = row;
            }
        }

        for (int i = y.Count - 1; i >= 0; i--)
        {
            matrix[i][y.Count] /= matrix[i][i];
            for (int j = i - 1; j >= 0; j--)
            {
                matrix[j][y.Count] -= matrix[j][i] * matrix[i][y.Count];
            }
        }

        List<double> result = new();
        for (int i = 0; i < y.Count; i++)
        {
            result.Add(matrix[i][y.Count]);
        }

        return result;
    }

    public double GetValue()
    {
        List<double> coe = FitPolynomial(_numbers.ToList());
        double result = 0;
        for (int i = 0; i < coe.Count; i++)
        {
            result += coe[i] * Math.Pow(_numbers.Count+1, coe.Count - 1 - i);
        }
        return result;
    }

    public void ResetIndicator()
    {
        _numbers = new();
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "Numbers", _numbers.ToArray() }
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        if (intermediateData.TryGetValue("Numbers", out var numbersValue) &&
            numbersValue is JArray numbers)
        {
            _numbers = new Queue<double>(numbers.ToObject<List<double>>()!);
        }
    }
}