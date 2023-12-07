using Model.SupportedDataFormats.Base;

namespace Model.SupportedStorages.Base;
public abstract class Series
{
    public string Name { get; protected set; } = null!;
    protected readonly Dictionary<string, Data> _series = [];

    public abstract bool ContainsID(string id);
    public abstract void Add(string id, Data data);
    public abstract Data Get(string id);
    public abstract void Remove(string id);
}
