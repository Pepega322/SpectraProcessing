namespace SpectraProcessing.Bll.Settings;

public record DataStorageSettings
{
    public required string DefaultDataSetName { get; init; }
}
