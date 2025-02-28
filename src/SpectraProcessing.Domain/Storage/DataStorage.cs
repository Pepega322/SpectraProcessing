using System.Collections;
using System.Collections.Concurrent;

namespace SpectraProcessing.Domain.Storage;

public class DataStorage<T> : IEnumerable<DataSet<T>>
{
    private readonly string defaultKey;
    private readonly ConcurrentDictionary<string, DataSet<T>> storage;
    public DataSet<T> DefaultSet => storage[defaultKey];
    public DataSet<T> this[string setKey] => storage[setKey];

    public DataStorage(string defaultKey)
    {
        this.defaultKey = defaultKey;
        storage = [];
        Add(defaultKey, new DataSet<T>(defaultKey));
    }

    public void Add(string key, DataSet<T> set)
    {
        if (storage.TryAdd(key, set)) return;
        key = GetNewSetKey(key);
        storage.TryAdd(key, set);
    }

    public bool ContainsKey(string key)
    {
        return storage.ContainsKey(key);
    }

    public bool RemoveThreadSafe(string key)
    {
        if (key != defaultKey)
        {
            return storage.TryRemove(key, out _);
        }

        storage[defaultKey] = new DataSet<T>(defaultKey);
        return true;
    }

    public void ClearThreadSafe()
    {
        storage.Clear();
        Add(defaultKey, new DataSet<T>(defaultKey));
    }

    private string GetNewSetKey(string setKey)
    {
        var i = 1;
        while (true)
        {
            var newSetKey = $"{setKey} ({i})";
            if (!storage.ContainsKey(newSetKey))
                return newSetKey;
            i++;
        }
    }

    public IEnumerator<DataSet<T>> GetEnumerator() => storage.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
