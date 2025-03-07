namespace SpectraProcessing.Controllers.Settings;

public record DataStorageSettings
{
    public required string DefaultDataSetName { get; init; }
}
