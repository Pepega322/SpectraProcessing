namespace Model.SupportedStorages.Base;
internal abstract class Storage
{
    public string Name { get; protected set; } = null!;
    protected readonly Dictionary<string, Series> _storage = [];

    public abstract bool ContainsID(string id);
    public abstract void Add(string id, Series series);
    public abstract Series Get(string id);
    public abstract void Remove(string id);
}
