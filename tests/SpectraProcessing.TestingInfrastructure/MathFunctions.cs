namespace SpectraProcessing.TestingInfrastructure;

public static class MathFunctions
{
    public static readonly (float X, float Y) RosenbrockMinimum = (1, 1);

    public static readonly (float X, float Y)[] HimmelblauMinimums =
    [
        (3, 2),
        (3.584428f, -1.848126f),
        (-2.805118f, 3.131312f),
        (-3.779310f, -3.283186f),
    ];

    public static float RosenbrockFunc(float x, float y)
    {
        return (1 - x) * (1 - x) + 100 * (y - x * x) * (y - x * x);
    }

    public static float HimmelblauFunc(float x, float y)
    {
        return (x * x + y - 11) * (x * x + y - 11) + (x + y * y - 7) * (x + y * y - 7);
    }
}
