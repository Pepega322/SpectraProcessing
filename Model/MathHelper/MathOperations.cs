using System.Numerics;

namespace Model.MathHelper;
internal class MathOperations {
    public static Func<float, float> GetLinearRegression(IList<float> xS, IList<float> yS) {
        var xSum = xS.Take(xS.Count).Sum();
        var x2Sum = xS.Take(xS.Count).Sum(e => e * e);
        var ySum = yS.Take(xS.Count).Sum();
        var xySum = 0f;
        for (var i = 0; i < xS.Count; i++)
            xySum += xS[i] * yS[i];

        var z = xS.Count * x2Sum - xSum * xSum;
        var a = (xS.Count * xySum - xSum * ySum) / z;
        var b = (ySum * x2Sum - xSum * xySum) / z;
        return x => a * x + b;
    }

    public static int GetClosestIndex<T>(IList<T> arr, T element) where T : INumber<T> {
        var left = 0;
        var right = arr.Count - 1;
        while (left < right) {
            var middle = (left + right) / 2;
            if (element <= arr[middle])
                right = middle;
            else left = middle + 1;
        }
        if (left == 0) return 0;
        var dLeft = element - arr[left - 1];
        var dRight = arr[left] - element;
        return dLeft <= dRight ? left - 1 : left;
    }

    public static float GetQuadrangleSquare(float baseLength1, float baseLength2, float heigth)
        => 0.5f * (baseLength1 + baseLength2) * heigth;
}