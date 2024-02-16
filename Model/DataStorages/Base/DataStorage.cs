using System.Collections;
using Model.DataFormats;

namespace Model.DataStorages;
public abstract class DataStorage : IEnumerable<KeyValuePair<string, DataSet>> {
    protected const string DefaultSetKey = "Default";
    protected Dictionary<string, DataSet> storage;
    public DataSet DefaultSet => storage[DefaultSetKey];

    public DataStorage() {
        storage = new Dictionary<string, DataSet>();
        AddDefaultSet();
    }

    protected abstract void AddDefaultSet();

    public abstract bool AddToDefaultSet(Data data);

    public bool ContainsSet(string setKey) => storage.ContainsKey(setKey);

    protected virtual bool AddSet(string setKey, DataSet set) {
        if (set is TreeDataSetNode) return false;
        storage.Add(setKey, set);
        return true;
    }

    public abstract bool Add(string setKey, DataSet set);

    public bool Remove(string setKey) => storage.Remove(setKey);

    public void Clear() {
        storage.Clear();
        AddDefaultSet();
    }

    public IEnumerator<KeyValuePair<string, DataSet>> GetEnumerator() {
        foreach (var pair in storage)
            yield return pair;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
