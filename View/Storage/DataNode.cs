using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Unsupported;
using Model.SupportedDataSources.Base;

namespace View.Storage;
public class DataNode : IComparable
{
    public readonly string Name;
    private readonly DataSource _source;
    private readonly SortedSet<Data> _dataSet;
    private readonly SortedSet<DataNode> _childrens;
    public DataNode? Parent { get; }
    public IEnumerable<Data> Data => _dataSet;
    public IEnumerable<DataNode> Childrens => _childrens;

    public DataNode(DataSource source, string name, DirectoryInfo directory, bool isAddSubdirs, DataNode? parent = null)
    {
        Name = name;
        _source = source;
        Parent = parent;
        _dataSet = new();
        Parallel.ForEach(directory.GetFiles(), AddData);
        _childrens = new();
        if (isAddSubdirs)
            Parallel.ForEach(directory.GetDirectories(), AddNode);
    }

    public DataNode(DataSource source, string name)
    {
        Name = name;
        _source = source;
        _dataSet = [];
        _childrens = [];
    }

    public void AppendData(Data data)
    {
        lock (_dataSet) _dataSet.Add(data);
    }

    public bool RemoveData(Data data)
    {
        lock (_dataSet) return _dataSet.Remove(data);
    }

    public bool DisconnectFromParent()
    {
        if (Parent == null) return false;
        return Parent._childrens.Remove(this);
    }

    private void AddData(FileInfo file)
    {
        var data = _source.ReadFile(file.FullName);
        if (data is not Undefined and not Empty)
            AppendData(data);
    }

    private void AddNode(DirectoryInfo directory)
    {
        var node = new DataNode(_source, directory.Name, directory, true, this);
        lock (_childrens) _childrens.Add(node);
    }

    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;
        if (obj is not DataNode node)
            throw new ArgumentException("Object is not DataNode");
        return CompareTo(node);
    }

    private int CompareTo(DataNode node) => Name.CompareTo(node.Name);
}
