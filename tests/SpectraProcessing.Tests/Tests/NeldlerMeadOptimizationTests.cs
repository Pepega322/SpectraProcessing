using FluentAssertions;
using SpectraProcessing.Domain.MathModeling;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.TestingInfrastructure;
using Xunit;

namespace SpectraProcessing.Tests.Tests;

public class NeldlerMeadOptimizationTests
{
    private static readonly OptimizationSettings Settings = new();

    [Theory]
    [MemberData(nameof(GetRosenbrockFuncStarts))]
    public async Task Optimize_RosenbrockFunc_Success(VectorN start)
    {
        //Act
        var actual = await NelderMead.Optimization(start, Func, Settings);

        //Assert
        actual[0].Should().BeApproximately(MathFunctions.RosenbrockMinimum.X, 1e-6);
        actual[1].Should().BeApproximately(MathFunctions.RosenbrockMinimum.Y, 1e-6);

        return;

        double Func(VectorN vector) => MathFunctions.RosenbrockFunc(vector[0], vector[1]);
    }

    [Theory]
    [MemberData(nameof(GetHimmelblauFuncStartsAndExpectedMinimums))]
    public async Task Optimize_HimmelblauFunc_Success(VectorN start, VectorN expectedMinimum)
    {
        //Act
        var actual = await NelderMead.Optimization(start, Func, Settings);

        //Assert
        actual[0].Should().BeApproximately(expectedMinimum[0], 1e-6);
        actual[1].Should().BeApproximately(expectedMinimum[1], 1e-6);

        return;

        double Func(VectorN vector) => MathFunctions.HimmelblauFunc(vector[0], vector[1]);
    }

    public static TheoryData<VectorN> GetRosenbrockFuncStarts()
        => new(
        [
            new VectorN([1, 1]),
            new VectorN([10, 10]),
            new VectorN([-10, -10]),
            new VectorN([10, -10]),
            new VectorN([-10, 10]),
        ]);

    public static TheoryData<VectorN, VectorN> GetHimmelblauFuncStartsAndExpectedMinimums()
    {
        var data = new TheoryData<VectorN, VectorN>();

        (VectorN, VectorN)[] startsAndExpectedMinimums =
        [
            (new VectorN([3, 2]), new VectorN([3, 2])),

            (new VectorN([3.584428, -1.848126]), new VectorN([3.584428, -1.848126])),

            (new VectorN([-2.805118, 3.131312]), new VectorN([-2.805118, 3.131312])),

            (new VectorN([-3.779310, -3.283186]), new VectorN([-3.779310, -3.283186])),
        ];

        foreach (var (start, min) in startsAndExpectedMinimums)
        {
            data.Add(start, min);
        }

        return data;
    }
}
