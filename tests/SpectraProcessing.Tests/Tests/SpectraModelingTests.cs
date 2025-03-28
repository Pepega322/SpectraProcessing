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

        var estimates = ModelSpectras.GaussPeaks;

        var expected = ModelSpectras.GaussPeaks;

        //Act
        var actual = await spectra.FitPeaks(estimates, OptimizationSettings.Default);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task FitPeaks_Lorentz_Success()
    {
        //Arrange
        var spectra = ModelSpectras.Lorentz;

        var estimates = ModelSpectras.LorentzPeaks;

        var expected = ModelSpectras.LorentzPeaks;

        //Act
        var actual = await spectra.FitPeaks(estimates, OptimizationSettings.Default);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task FitPeaks_GaussAndLorentz_Success()
    {
        //Arrange
        var spectra = ModelSpectras.GaussAndLorentz;

        var estimates = ModelSpectras.GaussAndLorentzPeaks;

        var expected = ModelSpectras.GaussAndLorentzPeaks;

        //Act
        var actual = await spectra.FitPeaks(estimates, OptimizationSettings.Default);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
