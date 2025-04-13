using FluentAssertions;
using SpectraProcessing.Domain.MathModeling;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.TestingInfrastructure;
using Xunit;

namespace SpectraProcessing.Tests.Tests;

public class SpectraModelingTests
{
    private const float shiftPercentage = 0.1f;
    private static readonly OptimizationSettings OptimizationSettings = OptimizationSettings.Default;

    [Fact]
    public async Task FitPeaks_Gauss_Success()
    {
        //Arrange
        var spectra = ModelSpectras.Gauss;

        var expected = ModelSpectras.GaussPeaks.OrderBy(p => p.Center).ToArray();

        var actual = ModelSpectras.GaussPeaks.Select(x => Shift(x.Copy())).ToArray();

        //Act
        await spectra.FitPeaks(actual, OptimizationSettings);

        //Assert
        for (var i = 0; i < actual.Length; i++)
        {
            actual[i].Center.Should().BeApproximately(expected[i].Center, 1);
            actual[i].HalfWidth.Should().BeApproximately(expected[i].HalfWidth, 1);
            actual[i].Amplitude.Should().BeApproximately(expected[i].Amplitude, 1);
            actual[i].GaussianContribution.Should().BeApproximately(expected[i].GaussianContribution, 1);
        }
    }

    [Fact]
    public async Task FitPeaks_Lorentz_Success()
    {
        //Arrange
        var spectra = ModelSpectras.Lorentz;

        var expected = ModelSpectras.LorentzPeaks.OrderBy(p => p.Center).ToArray();

        var actual = ModelSpectras.LorentzPeaks.Select(x => Shift(x.Copy())).ToArray();

        //Act
        await spectra.FitPeaks(actual, OptimizationSettings);

        //Assert
        for (var i = 0; i < actual.Length; i++)
        {
            actual[i].Center.Should().BeApproximately(expected[i].Center, 1);
            actual[i].HalfWidth.Should().BeApproximately(expected[i].HalfWidth, 1);
            actual[i].Amplitude.Should().BeApproximately(expected[i].Amplitude, 1);
            actual[i].GaussianContribution.Should().BeApproximately(expected[i].GaussianContribution, 1);
        }
    }

    [Fact]
    public async Task FitPeaks_GaussAndLorentz_Success()
    {
        //Arrange
        var spectra = ModelSpectras.GaussAndLorentz;

        var expected = ModelSpectras.GaussAndLorentzPeaks.OrderBy(p => p.Center).ToArray();

        var actual = ModelSpectras.GaussAndLorentzPeaks.Select(x => Shift(x.Copy())).ToArray();

        //Act
        await spectra.FitPeaks(actual, OptimizationSettings);

        //Assert
        for (var i = 0; i < actual.Length; i++)
        {
            actual[i].Center.Should().BeApproximately(expected[i].Center, 1);
            actual[i].HalfWidth.Should().BeApproximately(expected[i].HalfWidth, 1);
            actual[i].Amplitude.Should().BeApproximately(expected[i].Amplitude, 1);
            actual[i].GaussianContribution.Should().BeApproximately(expected[i].GaussianContribution, 1);
        }
    }

    private static PeakData Shift(PeakData peak)
    {
        peak.Center *= 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage;
        peak.Amplitude *= 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage;
        peak.HalfWidth *= 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage;

        return peak;
    }
}
