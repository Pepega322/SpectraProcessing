using System.Collections;

namespace Domain;
public class DataStorage<TData> : IEnumerable<KeyValuePair<string, DataSet<TData>>> where TData : Data {
    private readonly string defaultKey;
    private readonly Dictionary<string, DataSet<TData>> storage;
    public DataSet<TData> DefaultSet => storage[defaultKey];
    public DataSet<TData> this[string setKey] => storage[setKey];

    public DataStorage(string defaultKey) {
        this.defaultKey = defaultKey;
        storage = [];
        storage.Add(defaultKey, new DataSet<TData>(defaultKey));
    }

    public void AddThreadSafe(string key, DataSet<TData> set) {
        lock (storage) {
            if (!storage.TryAdd(key, set)) 
                storage.Add(GetNewSetKey(key), set);
        }
    }

    public bool ContainsKeyThreadSafe(string key) {
        bool result;
        lock (storage)
            result = storage.ContainsKey(key);
        return result;
    }

    public bool RemoveThreadSafe(string key) {
        lock (storage) {
            if (key == defaultKey) {
                storage[defaultKey] = new DataSet<TData>(defaultKey);
                return true;
            }
            return storage.Remove(key);
        }
    }

    public void ClearThreadSafe() {
        lock (storage) {
            storage.Clear();
            storage.Add(defaultKey, new DataSet<TData>(defaultKey));
        }
    }

    private string GetNewSetKey(string setKey) {
        var i = 1;
        while (true) {
            var newSetKey = $"{setKey} ({i})";
            if (!storage.ContainsKey(newSetKey))
                return newSetKey;
            i++;
        }
    }

    public IEnumerator<KeyValuePair<string, DataSet<TData>>> GetEnumerator() {
        return storage.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
