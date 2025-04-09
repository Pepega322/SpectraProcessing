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

    [Fact]
    public async Task FitPeaks_Gauss_Success()
    {
        //Arrange
        var spectra = ModelSpectras.Gauss;

        var expected = ModelSpectras.GaussPeaks;

        var actual = ModelSpectras.GaussPeaks.Select(x => Shift(x.Copy())).ToArray();

        //Act
        await spectra.FitPeaks(actual, OptimizationSettings.Default);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task FitPeaks_Lorentz_Success()
    {
        //Arrange
        var spectra = ModelSpectras.Lorentz;

        var expected = ModelSpectras.LorentzPeaks;

        var actual = ModelSpectras.LorentzPeaks.Select(x => Shift(x.Copy())).ToArray();

        //Act
        await spectra.FitPeaks(actual, OptimizationSettings.Default);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task FitPeaks_GaussAndLorentz_Success()
    {
        //Arrange
        var spectra = ModelSpectras.GaussAndLorentz;

        var expected = ModelSpectras.GaussAndLorentzPeaks;

        var actual = ModelSpectras.GaussAndLorentzPeaks.Select(x => Shift(x.Copy())).ToArray();

        //Act
        await spectra.FitPeaks(actual, OptimizationSettings.Default);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private static PeakData Shift(PeakData peak, float shiftPercentage = shiftPercentage)
    {
        peak.Center *= 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage;
        peak.Amplitude *= 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage;
        peak.HalfWidth *= 1 + (Random.Shared.Next() % 2 == 0 ? 1 : -1) * shiftPercentage / 2;

        return peak;
    }
}
