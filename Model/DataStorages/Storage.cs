using System.Collections;
using Model.DataFormats;

namespace Model.DataStorages;
public abstract class Storage : IEnumerable<KeyValuePair<string, Set>> {
    protected const string DefaultSetKey = "Default";
    protected Dictionary<string, Set> storage;
    public Set DefaultSet => storage[DefaultSetKey];

    public Storage() {
        storage = new Dictionary<string, Set>();
        AddDefaultSet();
    }

    public abstract bool AddToDefaultSet(Data data);

    public bool ContainsSet(string setKey) => storage.ContainsKey(setKey);

    public bool RemoveSet(string setKey) => storage.Remove(setKey);

    public void Clear() {
        storage.Clear();
        AddDefaultSet();
    }

    protected abstract void AddDefaultSet();

    public IEnumerator<KeyValuePair<string, Set>> GetEnumerator() {
        foreach (var pair in storage)
            yield return pair;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
