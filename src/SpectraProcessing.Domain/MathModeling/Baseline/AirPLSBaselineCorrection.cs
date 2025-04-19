using SpectraProcessing.Domain.Models.MathModeling.Baseline;
using SpectraProcessing.Domain.Models.MathModeling.Common;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Domain.MathModeling.Baseline;

public static class AirPLS
{
    public static Task CorrectBaseline(SpectraData spectra, AirPLSBaselineCorrectionModel model)
    {
        return Task.Run(() => CorrectBaselineInternal(spectra, model));
    }

    private static void CorrectBaselineInternal(SpectraData spectra, AirPLSBaselineCorrectionModel model)
    {
        var yValues = new VectorNRefStruct(spectra.Points.Y);
        var weight = new VectorNRefStruct(stackalloc float[yValues.Dimension]);
    }

    private static int GetDTDMatrixValue(int row, int column, int size)
    {

    }
}
