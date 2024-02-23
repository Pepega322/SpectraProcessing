using Model.DataFormats;

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

    public bool ContainsSubset(string subsetName)
        => subsets.Where(s => s.Name == subsetName).Any();

    public bool AddSubset(TreeDataSetNode subset) {
        bool result;
        lock (subsets) result = subsets.Add(subset);
        if (result) IncreaseCount(subset.DataCount);
        return result;
    }

    public abstract TreeDataSetNode CopyBranchStructure(string rootName, out Dictionary<TreeDataSetNode, TreeDataSetNode> referenceToCopy);

    public bool DisconnectFromParent() {
        if (Parent == null) return false;
        var result = Parent.subsets.Remove(this);
        if (result) Parent.DecreaseCount(DataCount);
        return result;
    }

    private void DecreaseCount(int num = 1) {
        lock (this) DataCount -= num;
        Parent?.DecreaseCount(num);
    }

    private void IncreaseCount(int num = 1) {
        lock (this) DataCount += num;
        Parent?.IncreaseCount(num);
    }
}
