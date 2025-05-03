namespace SpectraProcessing.Bll.Models.Settings;

internal sealed record PeakProcessingSettings
{
    public int NedlerMeadMaxIterationsCount { get; init; }

    public int NedlerMeadRepeatsCount { get; init; }

    public int? NedlerMeadMaxConsecutiveShrinks { get; init; }

    public float? NedlerMeadMinDeltaPercentageBetweenIterations { get; init; }

    public int? NedlerMeadMaxIterationsWithDeltaLessThanMin { get; init; }

    public float? NedlerMeadAbsoluteValue { get; init; }
}
