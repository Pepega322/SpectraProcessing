using Model.DataFormats;
using System.Collections;

namespace Model.DataStorages;
public abstract class DataSet : IEnumerable<Data> {
    protected HashSet<Data> set;
    public string Name { get; protected set; } = null!;

    public DataSet(string name) {
        Name = name;
        set = new HashSet<Data>();
    }

    protected virtual bool AddToSet(Data data) {
        bool result;
        lock (set) result = set.Add(data);
        return result;
    }

    protected virtual bool RemoveFromSet(Data data) {
        bool result;
        lock (set) result = set.Remove(data);
        return result;
    }

    public abstract bool Add(Data data);

    public abstract bool Remove(Data data);

    public IEnumerator<Data> GetEnumerator() {
        foreach (var data in set)
            yield return data;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
