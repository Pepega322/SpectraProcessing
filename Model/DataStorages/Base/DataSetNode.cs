﻿using Model.DataFormats.Base;
using Model.DataSources.Base;

namespace Model.DataStorages.Base;
public abstract class DataSetNode : IComparable
{
    protected readonly SortedSet<Data> _dataSet = null!;
    protected readonly SortedSet<DataSetNode> _nodes = null!;

    public string Name { get; protected set; } = null!;
    public DataSetNode? Parent { get; }
    public int DataCount { get; protected set; }
    public IEnumerable<Data> Data => _dataSet;
    public IEnumerable<DataSetNode> Nodes => _nodes;

    public DataSetNode(string name, DataSetNode? parent = null)
    {
        Name = name;
        Parent = parent;
        _dataSet = [];
        _nodes = [];
    }

    public bool AddData(Data data)
    {
        bool result;
        lock (_dataSet) result = _dataSet.Add(data);
        if (result) IncreaseCount();
        return result;
    }

    public bool RemoveData(Data data)
    {
        bool result;
        lock (_dataSet) result = _dataSet.Remove(data);
        if (result) Parent?.DecreaseCount();
        return result;
    }

    public bool DisconnectFromParent()
    {
        if (Parent == null) return false;
        var result = Parent._nodes.Remove(this);
        if (result) Parent.DecreaseCount(DataCount);
        return result;
    }

    protected abstract void InitializeData(DataSource source, string pathForSource);

    protected abstract void InitializeNodes(DataSource source, string pathForSource);

    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;
        if (obj is not DataSetNode node)
            throw new ArgumentException("Object is not DataNode");
        return CompareTo(node);
    }

    protected virtual int CompareTo(DataSetNode node) => Name.CompareTo(node.Name);

    private void DecreaseCount(int num = 1)
    {
        lock (this) DataCount -= num;
        Parent?.DecreaseCount(num);
    }

    private void IncreaseCount(int num = 1)
    {
        lock (this) DataCount += num;
        Parent?.IncreaseCount(num);
    }
}
