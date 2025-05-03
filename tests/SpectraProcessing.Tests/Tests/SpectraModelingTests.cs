using FluentAssertions;
using SpectraProcessing.Domain.MathModeling.Peaks;
using SpectraProcessing.Domain.Models.MathModeling.Peaks;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.TestingInfrastructure;
using Xunit;

namespace SpectraProcessing.Tests.Tests;

public class SpectraModelingTests
{
    private const float shiftPercentage = 0.05f;

    private readonly NedlerMeadSettings settings = new();

    [Fact(Skip = "TODO")]
    public async Task FitPeaks_Gauss_Success()
    {
        //Arrange
        var spectra = ModelSpectras.Gauss;

        var expected = ModelSpectras.GaussPeaks;

        var actual = ModelSpectras.GaussPeaks.Select(x => Shift(x.Copy())).ToArray();

        //Act
        await spectra.FitPeaks(actual, settings);

        //Assert
        AssertAreEqual(actual, expected);
    }

    [Fact(Skip = "TODO")]
    public async Task FitPeaks_Lorentz_Success()
    {
        //Arrange
        var spectra = ModelSpectras.Lorentz;

        var expected = ModelSpectras.LorentzPeaks;

        var actual = ModelSpectras.LorentzPeaks.Select(x => Shift(x.Copy())).ToArray();

        //Act
        await spectra.FitPeaks(actual, settings);

        //Assert
        AssertAreEqual(actual, expected);
    }

    [Fact(Skip = "TODO")]
    public async Task FitPeaks_GaussAndLorentz_Success()
    {
        //Arrange
        var spectra = ModelSpectras.GaussAndLorentz;

        var expected = ModelSpectras.GaussAndLorentzPeaks;

        var actual = ModelSpectras.GaussAndLorentzPeaks.Select(x => Shift(x.Copy())).ToArray();

        //Act
        await spectra.FitPeaks(actual, settings);

        //Assert
        AssertAreEqual(actual, expected);
    }

    private static void AssertAreEqual(IReadOnlyList<PeakData> actual, IReadOnlyList<IReadOnlyPeakData> expected)
    {
        actual.Count.Should().Be(expected.Count);

        expected = expected.OrderBy(p => p.Center).ToArray();
        actual = actual.OrderBy(p => p.Center).ToArray();

        actual.Should().BeEquivalentTo(expected);

        // for (var i = 0; i < actual.Count; i++)
        // {
        //     actual[i].Center.Should()
        //         .BeApproximately(expected[i].Center, expected[i].Center * 0.05f);
        //     actual[i].HalfWidth.Should()
        //         .BeApproximately(expected[i].HalfWidth, expected[i].HalfWidth * 0.2f);
        //     actual[i].Amplitude.Should()
        //         .BeApproximately(expected[i].Amplitude, expected[i].Amplitude * 0.05f);
        //     actual[i].GaussianContribution.Should()
        //         .BeApproximately(expected[i].GaussianContribution, 0.1f);
        // }
    }

    private static PeakData Shift(PeakData peak)
    {
        peak.HalfWidth *= 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage;

        peak.Center += 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * peak.HalfWidth / 2;

        peak.Amplitude *= 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage;
        peak.GaussianContribution *= 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage;

        return peak;
    }
}
