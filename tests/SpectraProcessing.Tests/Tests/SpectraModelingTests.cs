using FluentAssertions;
using SpectraProcessing.Domain.MathModeling;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.TestingInfrastructure;
using Xunit;

namespace SpectraProcessing.Tests.Tests;

public class SpectraModelingTests
{
    [Fact]
    public async Task FitPeaks_Gauss_Success()
    {
        //Arrange
        var spectra = ModelSpectras.Gauss;

        var expected = ModelSpectras.GaussPeaks;

        var actual = ModelSpectras.GaussPeaks.Select(x => x.Copy()).ToArray();

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

        var actual = ModelSpectras.LorentzPeaks.Select(x => x.Copy()).ToArray();

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

        var actual = ModelSpectras.GaussAndLorentzPeaks.Select(x => x.Copy()).ToArray();

        //Act
        await spectra.FitPeaks(actual, OptimizationSettings.Default);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
