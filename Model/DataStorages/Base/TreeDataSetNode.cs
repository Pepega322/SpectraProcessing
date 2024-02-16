using Model.DataFormats;
using Model.DataSources;

namespace Model.DataStorages;
public abstract class TreeDataSetNode : DataSet {
    protected readonly HashSet<TreeDataSetNode> subsets = null!;
    public TreeDataSetNode? Parent { get; }
    public int DataCount { get; protected set; }
    public IEnumerable<TreeDataSetNode> Subsets => subsets;

    public TreeDataSetNode(string name, TreeDataSetNode? parent = null)
        : base(name) {
        Parent = parent;
        subsets = [];
    }

    protected override bool AddToSet(Data data) {
        bool result;
        lock (set) result = set.Add(data);
        if (result) IncreaseCount();
        return result;
    }

    protected override bool RemoveFromSet(Data data) {
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

    protected abstract void ReadData(DataReader reader, string path);

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
