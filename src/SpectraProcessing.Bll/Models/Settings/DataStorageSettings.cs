namespace SpectraProcessing.Bll.Models.Settings;

public record DataStorageSettings
{
    public required string DefaultDataSetName { get; init; }
}
