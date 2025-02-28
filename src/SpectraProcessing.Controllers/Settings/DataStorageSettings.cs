namespace SpectraProcessing.Controllers.Settings;

public record DataStorageSettings
{
    public string DefaultDataSetName { get; init; } = null!;
}
