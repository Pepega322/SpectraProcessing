using System.Collections;
using System.Collections.Concurrent;
using SpectraProcessing.Models.Collections.Keys;

namespace SpectraProcessing.Models.Collections;

public class DataStorage<TKey, TValue> : IReadOnlyCollection<DataSet<TValue>>
    where TKey : INamedKey
{
    private readonly string defaultSetName;

    private readonly ConcurrentDictionary<TKey, DataSet<TValue>> storage;

    public DataSet<TValue> DefaultSet { get; private set; }

    public DataSet<TValue> this[TKey setKey] => storage[setKey];

    public DataStorage(string defaultSetName)
    {
        storage = [];
        this.defaultSetName = defaultSetName;
        DefaultSet = new DataSet<TValue>(defaultSetName);
    }

    public void Add(TKey key, DataSet<TValue> set)
    {
        storage.TryAdd(key, set);
    }

    public bool ContainsKey(TKey key)
    {
        return storage.ContainsKey(key);
    }

    public bool RemoveThreadSafe(TKey key)
    {
        if (key.Name.Equals(defaultSetName))
        {
            DefaultSet = new DataSet<TValue>(defaultSetName);
        }

        return storage.TryRemove(key, out _);
    }

    public void ClearThreadSafe()
    {
        storage.Clear();

        DefaultSet = new DataSet<TValue>(defaultSetName);
    }

    public int Count => storage.Count;

    public IEnumerator<DataSet<TValue>> GetEnumerator() => storage.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
