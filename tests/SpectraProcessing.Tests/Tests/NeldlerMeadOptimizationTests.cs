using FluentAssertions;
using SpectraProcessing.Domain.MathModels;
using SpectraProcessing.Domain.Models.MathModels;
using SpectraProcessing.TestingInfrastructure;
using Xunit;

namespace SpectraProcessing.Tests.Tests;

public class NeldlerMeadOptimizationTests
{
    private static readonly NelderMeadOptimizationSettings Settings = new()
    {
    };

    [Theory]
    [MemberData(nameof(GetRosenbrockFuncStarts))]
    public async Task Optimize_RosenbrockFunc_Success(VectorN start)
    {
        //Act
        var actual = await NelderMeadOptimization.Optimize(start, Func, Settings);

        //Assert
        var expected = new VectorN([MathFunctions.RosenbrockMinimum.X, MathFunctions.RosenbrockMinimum.Y]);

        (actual == expected).Should().BeTrue();

        return;

        double Func(VectorN vector) => MathFunctions.RosenbrockFunc(vector[0], vector[1]);
    }

    [Theory]
    [MemberData(nameof(GetHimmelblauFuncStartsAndExpectedMinimums))]
    public async Task Optimize_HimmelblauFunc_Success(VectorN start, VectorN expectedMinimum)
    {
        //Act
        var actual = await NelderMeadOptimization.Optimize(start, Func, Settings);

        //Assert
        (actual == expectedMinimum).Should().BeTrue();

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
