using Microsoft.Extensions.Options;
using SpectraProcessing.Bll.Models.Settings;
using SpectraProcessing.Bll.Monitors.Interfaces;

namespace SpectraProcessing.Bll.Monitors;

internal class PeakProcessingSettingsMonitor : IPeakProcessingSettingsMonitor
{
    public int NedlerMeadMaxIterationsCount { get; private set; }

    public int NedlerMeadRepeatsCount { get; private set; }

    public int? NedlerMeadMaxConsecutiveShrinks { get; private set; }

    public float? NedlerMeadMinDeltaPercentageBetweenIterations { get; private set; }

    public int? NedlerMeadMaxIterationsWithDeltaLessThanMin { get; private set; }

    public float? NedlerMeadMinAbsoluteValue { get; private set; }

    public PeakProcessingSettingsMonitor(IOptionsMonitor<PeakProcessingSettings> monitor)
    {
        UpdateSettings(monitor.CurrentValue);
        monitor.OnChange(UpdateSettings);
    }

    private void UpdateSettings(PeakProcessingSettings settings)
    {
        NedlerMeadMaxIterationsCount = settings.NedlerMeadMaxIterationsCount;
        NedlerMeadRepeatsCount = settings.NedlerMeadRepeatsCount;
        NedlerMeadMaxConsecutiveShrinks = settings.NedlerMeadMaxConsecutiveShrinks;
        NedlerMeadMinDeltaPercentageBetweenIterations = settings.NedlerMeadMinDeltaPercentageBetweenIterations;
        NedlerMeadMaxIterationsWithDeltaLessThanMin = settings.NedlerMeadMaxIterationsWithDeltaLessThanMin;
        NedlerMeadMinAbsoluteValue = settings.NedlerMeadAbsoluteValue;
    }
}
