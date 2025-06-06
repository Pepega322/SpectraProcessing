using System.Numerics;
using SpectraProcessing.Domain.Collections;

namespace SpectraProcessing.Domain.MathModeling.Common;

public static class RegressionAnalysis
{
    public static Func<float, float> GetLinearRegression(SpectraPoints points)
    {
        var xSum = points.X.Sum();
        var x2Sum = points.X.Sum(e => e * e);
        var ySum = points.Y.Sum();
        var xySum = 0f;
        for (var i = 0; i < points.Count; i++)
        {
            xySum += points.X[i] * points.Y[i];
        }

        var z = points.Count * x2Sum - xSum * xSum;

        var a = (points.Count * xySum - xSum * ySum) / z;

        var b = (ySum * x2Sum - xSum * xySum) / z;

        return x => a * x + b;
    }

    public static Func<T, T> GetLinearRegression<T>(Point<T> p1, Point<T> p2) where T : struct, INumber<T>
    {
        var a = (p2.Y - p1.Y) / (p2.X - p1.X);

        var b = p1.Y - a * p1.X;

        return x => a * x + b;
    }

    public static int ClosestIndexBinarySearch<T>(this IReadOnlyList<T> arr, T element) where T : INumber<T>
    {
        var left = 0;
        var right = arr.Count - 1;

        while (left < right)
        {
            var middle = (left + right) / 2;

            if (element <= arr[middle])
            {
                right = middle;
            }
            else
            {
                left = middle + 1;
            }
        }

        if (left == 0)
        {
            return 0;
        }

        var dLeft = element - arr[left - 1];
        var dRight = arr[left] - element;
        return dLeft <= dRight ? left - 1 : left;
    }

    public static float GetQuadrangleSquare(float baseLength1, float baseLength2, float height)
        => 0.5f * (baseLength1 + baseLength2) * height;
}
