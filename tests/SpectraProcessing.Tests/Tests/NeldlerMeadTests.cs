using FluentAssertions;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.MathModeling.Peaks;
using SpectraProcessing.Domain.Models.MathModeling.Common;
using SpectraProcessing.Domain.Models.MathModeling.Peaks;
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
        var model = new NedlerMeadModel
        {
            Start = start,
            Constraints = [],
            BufferSize = 0,
            Settings = new NedlerMeadSettings(),
        };

        var actual = await NelderMead.GetOptimized(model, Func);

        //Assert
        actual[0].Should().BeApproximately(
            MathFunctions.RosenbrockMinimum.X,
            1e-3f);

        actual[1].Should().BeApproximately(
            MathFunctions.RosenbrockMinimum.Y,
            1e-3f);

        return;

        float Func(VectorNRefStruct vector, Span<float> buffer) => MathFunctions.RosenbrockFunc(vector[0], vector[1]);
    }

    [Theory]
    [MemberData(nameof(GetHimmelblauFuncStartsAndExpectedMinimums))]
    public async Task Optimize_HimmelblauFunc_Success(VectorN start, VectorN expectedMinimum)
    {
        //Act
        var model = new NedlerMeadModel
        {
            Start = start,
            Constraints = [],
            BufferSize = 0,
            Settings = new NedlerMeadSettings(),
        };

        var actual = await NelderMead.GetOptimized(model, Func);

        //Assert
        actual[0].Should().BeApproximately(
            expectedMinimum[0],
            1e-3f);

        actual[1].Should().BeApproximately(
            expectedMinimum[1],
            1e-3f);

        return;

        float Func(VectorNRefStruct vector, Span<float> buffer) => MathFunctions.HimmelblauFunc(vector[0], vector[1]);
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
        var shifted = Enumerable.Range(0, vector.Dimension)
            .Select(d => vector[d] * (1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage))
            .ToArray();

        return new VectorN(shifted);
    }
}
