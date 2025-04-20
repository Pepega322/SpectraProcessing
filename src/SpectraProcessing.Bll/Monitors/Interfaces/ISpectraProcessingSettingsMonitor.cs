namespace SpectraProcessing.Bll.Monitors.Interfaces;

public interface ISpectraProcessingSettingsMonitor
{
    int AirPLSIterations { get; }

    float AirPLSMaxPeaksWidth { get; set; }
}
