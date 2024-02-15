using Model.DataFormats;
using Model.DataSources;

namespace Model.DataStorages;
public abstract class TreeSet : Set {
    protected readonly SortedSet<TreeSet> subsets = null!;
    public TreeSet? Parent { get; }
    public int DataCount { get; protected set; }
    public IEnumerable<TreeSet> Subsets => subsets;

    public TreeSet(string name, TreeSet? parent = null)
        : base(name) {
        Parent = parent;
        subsets = [];
    }

    public override bool Add(Data data) {
        if (data is not Spectra) return false;
        bool result;
        lock (set) result = set.Add(data);
        if (result) IncreaseCount();
        return result;
    }

    public override bool Remove(Data data) {
        bool result;
        lock (set) result = set.Remove(data);
        if (result) Parent?.DecreaseCount();
        return result;
    }

    public bool DisconnectFromParent() {
        if (Parent == null) return false;
        var result = Parent.subsets.Remove(this);
        if (result) Parent.DecreaseCount(DataCount);
        return result;
    }

    protected abstract void AddData(DataReader reader, string path);

    protected abstract void AddSubnodes(DataReader reader, string path);

    private void DecreaseCount(int num = 1) {
        lock (this) DataCount -= num;
        Parent?.DecreaseCount(num);
    }

    private void IncreaseCount(int num = 1) {
        lock (this) DataCount += num;
        Parent?.IncreaseCount(num);
    }
}
