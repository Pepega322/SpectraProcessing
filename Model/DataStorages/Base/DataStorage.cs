using System.Collections;
using Model.DataFormats;

namespace Model.DataStorages;
public abstract class DataStorage : IEnumerable<KeyValuePair<string, DataSet>> {
    protected const string DefaultSetKey = "Default";
    protected Dictionary<string, DataSet> storage;
    public DataSet DefaultSet => storage[DefaultSetKey];

    public DataSet this[string setKey] => storage[setKey];

    public DataStorage() {
        storage = new Dictionary<string, DataSet>();
        AddDefaultSet();
    }

    protected abstract void AddDefaultSet();

    public abstract bool AddToDefaultSet(Data data);

    public bool ContainsSet(string setKey) => storage.ContainsKey(setKey);

    protected virtual bool AddSet(string setKey, DataSet set) {
        if (set is TreeDataSetNode or PlotSet)
            throw new ArgumentException(nameof(set) + "isn't data set");
        storage.Add(setKey, set);
        return true;
    }

    public abstract bool Add(string setKey, DataSet set);

    public bool Remove(string setKey) => setKey != DefaultSetKey ? storage.Remove(setKey) : false;

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
