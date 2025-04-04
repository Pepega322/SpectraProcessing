using FluentAssertions;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.MathModeling;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.TestingInfrastructure;
using Xunit;

namespace SpectraProcessing.Tests.Tests;

public class NeldlerMeadTests
{
    [Theory]
    [MemberData(nameof(GetRosenbrockFuncStarts))]
    public async Task Optimize_RosenbrockFunc_Success(VectorN start)
    {
        //Act
        var actual = await NelderMead.GetOptimized(start, Func, OptimizationSettings.Default);

        //Assert
        actual.Values[0].Should().BeApproximately(
            MathFunctions.RosenbrockMinimum.X,
            ComparisonsExtensions.DoubleTolerance);

        actual.Values[1].Should().BeApproximately(
            MathFunctions.RosenbrockMinimum.Y,
            ComparisonsExtensions.DoubleTolerance);

        return;

        double Func(IReadOnlyVectorN vector) => MathFunctions.RosenbrockFunc(vector.Values[0], vector.Values[1]);
    }

    [Theory]
    [MemberData(nameof(GetHimmelblauFuncStartsAndExpectedMinimums))]
    public async Task Optimize_HimmelblauFunc_Success(VectorN start, VectorN expectedMinimum)
    {
        //Act
        var actual = await NelderMead.GetOptimized(start, Func, OptimizationSettings.Default);

        //Assert
        actual.Values[0].Should().BeApproximately(
            expectedMinimum[0],
            ComparisonsExtensions.DoubleTolerance);

        actual.Values[1].Should().BeApproximately(
            expectedMinimum[1],
            ComparisonsExtensions.DoubleTolerance);

        return;

        double Func(IReadOnlyVectorN vector) => MathFunctions.HimmelblauFunc(vector.Values[0], vector.Values[1]);
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
