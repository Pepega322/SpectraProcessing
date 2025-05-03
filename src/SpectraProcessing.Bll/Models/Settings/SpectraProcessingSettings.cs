namespace SpectraProcessing.Bll.Models.Settings;

internal sealed record SpectraProcessingSettings
{
    public int AirPLSIterations { get; init; }

    public float AirPLSMaxPeaksWidth { get; init; }
}
