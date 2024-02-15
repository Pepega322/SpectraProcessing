using Model.DataFormats;
using Model.DataSources;

namespace Model.DataStorages;
public abstract class DataSetNode : IComparable {
    protected readonly SortedSet<Data> dataSet = null!;
    protected readonly SortedSet<DataSetNode> nodes = null!;

    public string Name { get; protected set; } = null!;
    public DataSetNode? Parent { get; }
    public int DataCount { get; protected set; }
    public IEnumerable<Data> Data => dataSet;
    public IEnumerable<DataSetNode> Nodes => nodes;

    public DataSetNode(string name, DataSetNode? parent = null) {
        Name = name;
        Parent = parent;
        dataSet = [];
        nodes = [];
    }

    public bool Add(Data data) {
        bool result;
        lock (dataSet) result = dataSet.Add(data);
        if (result) IncreaseCount();
        return result;
    }

    public bool Remove(Data data) {
        bool result;
        lock (dataSet) result = dataSet.Remove(data);
        if (result) Parent?.DecreaseCount();
        return result;
    }

    public bool DisconnectFromParent() {
        if (Parent == null) return false;
        var result = Parent.nodes.Remove(this);
        if (result) Parent.DecreaseCount(DataCount);
        return result;
    }

    protected abstract void InitializeData(DataReader reader, string pathForReader);

    protected abstract void InitializeNodes(DataReader reader, string pathForReader);

    public int CompareTo(object? obj) {
        if (obj == null) return 1;
        if (obj is not DataSetNode node)
            throw new ArgumentException("Object is not DataNode");
        return CompareTo(node);
    }

    protected virtual int CompareTo(DataSetNode node) => Name.CompareTo(node.Name);

    private void DecreaseCount(int num = 1) {
        lock (this) DataCount -= num;
        Parent?.DecreaseCount(num);
    }

    private void IncreaseCount(int num = 1) {
        lock (this) DataCount += num;
        Parent?.IncreaseCount(num);
    }
}
