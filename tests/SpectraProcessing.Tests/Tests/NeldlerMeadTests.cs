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
        var model = new NedlerMeadOptimizationModel
        {
            Start = start,
            Constraints = [],
            BufferSize = 0,
            Settings = OptimizationSettings.Default,
        };

        var actual = await NelderMead.GetOptimized(model, Func);

        //Assert
        actual.Values[0].Should().BeApproximately(
            MathFunctions.RosenbrockMinimum.X,
            ComparisonsExtensions.FloatTolerance);

        actual.Values[1].Should().BeApproximately(
            MathFunctions.RosenbrockMinimum.Y,
            ComparisonsExtensions.FloatTolerance);

        return;

        float Func(VectorNRefStruct vector, Span<float> buffer)
            => MathFunctions.RosenbrockFunc(vector.Values[0], vector.Values[1]);
    }

    [Theory]
    [MemberData(nameof(GetHimmelblauFuncStartsAndExpectedMinimums))]
    public async Task Optimize_HimmelblauFunc_Success(VectorN start, VectorN expectedMinimum)
    {
        //Act
        var model = new NedlerMeadOptimizationModel
        {
            Start = start,
            Constraints = [],
            BufferSize = 0,
            Settings = OptimizationSettings.Default,
        };

        var actual = await NelderMead.GetOptimized(model, Func);

        //Assert
        actual.Values[0].Should().BeApproximately(
            expectedMinimum[0],
            ComparisonsExtensions.FloatTolerance);

        actual.Values[1].Should().BeApproximately(
            expectedMinimum[1],
            ComparisonsExtensions.FloatTolerance);

        return;

        float Func(VectorNRefStruct vector, Span<float> buffer)
            => MathFunctions.HimmelblauFunc(vector.Values[0], vector.Values[1]);
    }

    public static TheoryData<VectorN> GetRosenbrockFuncStarts()
        => new(
        [
            new VectorN([10, 10]),
            new VectorN([1, 1]),
            new VectorN([-10, -10]),
            new VectorN([10, -10]),
            new VectorN([-10, 10]),
        ]);

    public static TheoryData<VectorN, VectorN> GetHimmelblauFuncStartsAndExpectedMinimums()
    {
        VectorN[] expectedMinimums =
        [
            new([3, 2]),
            new([3.584428f, -1.848126f]),
            new([-2.805118f, 3.131312f]),
            new([-3.779310f, -3.283186f]),
        ];

        var data = new TheoryData<VectorN, VectorN>();

        var startsAndExpectedMinimums = expectedMinimums
            .Select(expected => (Shift(expected, 0.1f), expected));

        foreach (var (start, min) in startsAndExpectedMinimums)
        {
            data.Add(start, min);
        }

        return data;
    }

    private static VectorN Shift(VectorN vector, float shiftPercentage)
    {
        var shifted = vector.Values
            .Select(v => v * (1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage))
            .ToArray();

        return new VectorN(shifted);
    }
}
