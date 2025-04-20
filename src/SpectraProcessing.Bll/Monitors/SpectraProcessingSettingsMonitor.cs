using Microsoft.Extensions.Options;
using SpectraProcessing.Bll.Models.Settings;
using SpectraProcessing.Bll.Monitors.Interfaces;

namespace SpectraProcessing.Bll.Monitors;

internal sealed class SpectraProcessingSettingsMonitor : ISpectraProcessingSettingsMonitor
{
    public int AirPLSIterations { get; private set; }

    public float AirPLSMaxPeaksWidth { get; set; }

    public SpectraProcessingSettingsMonitor(IOptionsMonitor<SpectraProcessingSettings> monitor)
    {
        UpdateSettings(monitor.CurrentValue);
        monitor.OnChange(UpdateSettings);
    }

    private void UpdateSettings(SpectraProcessingSettings settings)
    {
        AirPLSIterations = settings.AirPLSIterations;
        AirPLSMaxPeaksWidth = settings.AirPLSMaxPeaksWidth;
    }
}
