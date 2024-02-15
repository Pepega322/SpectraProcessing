using Model.DataFormats;

namespace Model.DataStorages;
public abstract class Set  {
    protected HashSet<Data> set;

    public string Name { get; protected set; } = null!;
    public IEnumerable<Data> Data => set;

    public Set(string name) {
        Name = name;
        set = [];
    }

    public abstract bool Add(Data data);

    public abstract bool Remove(Data data);
}
