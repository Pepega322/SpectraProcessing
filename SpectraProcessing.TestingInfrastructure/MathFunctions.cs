namespace SpectraProcessing.TestingInfrastructure;

public static class MathFunctions
{
    public static readonly (double X, double Y) RosenbrockMinimum = (1, 1);

    public static readonly (double X, double Y)[] HimmelblauMinimums =
    [
        (3, 2),
        (3.584428, -1.848126),
        (-2.805118, 3.131312),
        (-3.779310, -3.283186),
    ];

    public static double RosenbrockFunc(double x, double y)
    {
        return (1 - x) * (1 - x) + 100 * (y - x * x) * (y - x * x);
    }

    public static double HimmelblauFunc(double x, double y)
    {
        return (x * x + y - 11) * (x * x + y - 11) + (x + y * y - 7) * (x + y * y - 7);
    }
}
