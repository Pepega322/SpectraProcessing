namespace SpectraProcessing.Bll.Monitors.Interfaces;

public interface IPeakProcessingSettingsMonitor
{
    int NedlerMeadMaxIterationsCount { get; }

    int NedlerMeadRepeatsCount { get; }

    int? NedlerMeadMaxConsecutiveShrinks { get; }

    float? NedlerMeadMinDeltaPercentageBetweenIterations { get; }

    int? NedlerMeadMaxIterationsWithDeltaLessThanMin { get; }

    float? NedlerMeadMinAbsoluteValue { get; }
}
