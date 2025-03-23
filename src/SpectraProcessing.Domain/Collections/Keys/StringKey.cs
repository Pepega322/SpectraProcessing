namespace SpectraProcessing.Domain.Collections.Keys;

public sealed class StringKey(string name) : INamedKey
{
    public string Name { get; } = name;

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is string str && string.Equals(Name, str) || obj is SpectraKey key && string.Equals(Name, key.Name);
    }

    public override int GetHashCode() => Name.GetHashCode();
}
