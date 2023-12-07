using Model.SupportedStorages.Base;

namespace Model.SupportedStorages;
internal class DataStorage : Storage
{
    public DataStorage(string name)
    {
        Name = name;
    }

    public override bool ContainsID(string id) => _storage.ContainsKey(id);
    public override void Add(string id, Series series) => _storage.Add(id, series);
    public override Series Get(string id) => _storage[id];
    public override void Remove(string id) => _storage.Remove(id);
}
