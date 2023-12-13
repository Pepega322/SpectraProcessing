using System.Collections;
using Model.DataFormats.Base;
using Model.DataSources.Base;

namespace Model.DataStorages.Base;
public abstract class DataStorage : IEnumerable<KeyValuePair<string, DataSetNode>>
{
    public const string DefaultDataSetKey = "Default";

    protected DataSource _source { get; set; } = null!;
    protected readonly Dictionary<string, DataSetNode> _storage = null!;

    public string Name { get; protected set; } = null!;
    public DataSetNode DefaultDataSet => _storage[DefaultDataSetKey];

    public DataStorage(DataSource source, string name)
    {
        Name = name;
        _source = source;
        _storage = [];
        AddDefaultSet();
    }

    protected abstract void AddDefaultSet();

    public void AddDataSet(string setKey, DataSetNode set) => _storage.Add(setKey, set);

    public void AddDataToDefaultSet(Data data) => DefaultDataSet.AddData(data);

    public bool ContainsSet(string setKey) => _storage.ContainsKey(setKey);

    public bool RemoveSet(string setKey) => _storage.Remove(setKey);

    public  IEnumerator<KeyValuePair<string, DataSetNode>> GetEnumerator()
    {
        foreach (var pair in _storage)
            yield return pair;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
