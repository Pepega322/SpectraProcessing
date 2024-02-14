using System.Collections;
using Model.DataFormats;
using Model.DataSources;

namespace Model.DataStorages;
public abstract class DataStorage : IEnumerable<KeyValuePair<string, DataSetNode>> {
    public const string DefaultDataSetKey = "Default";

    protected DataSource source { get; set; } = null!;
    protected Dictionary<string, DataSetNode> storage { get; private set; } = null!;

    public string Name { get; protected set; } = null!;
    public DataSetNode DefaultDataSet => storage[DefaultDataSetKey];

    public DataStorage(DataSource source, string name) {
        Name = name;
        this.source = source;
        storage = [];
        AddDefaultSet();
    }

    protected abstract void AddDefaultSet();

    public void AddDataSet(string setKey, DataSetNode set) => storage.Add(setKey, set);

    public bool AddToDefaultSet(Data data) {
        if (data is not Undefined or Сorrupted)
            return DefaultDataSet.Add(data);
        else return false;
    }

    public bool ContainsSet(string setKey) => storage.ContainsKey(setKey);

    public bool RemoveSet(string setKey) => storage.Remove(setKey);

    public IEnumerator<KeyValuePair<string, DataSetNode>> GetEnumerator() {
        foreach (var pair in storage)
            yield return pair;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
