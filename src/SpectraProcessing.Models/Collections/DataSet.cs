using System.Collections.Immutable;

namespace SpectraProcessing.Models.Collections;

public class DataSet<T>
{
    private readonly HashSet<T> set;

    private readonly HashSet<DataSet<T>> subsets;

    private int DataCount { get; set; }

    private DataSet<T>? Parent { get; set; }

    public string Name { get; protected set; }

    public IImmutableSet<T> Data
    {
        get
        {
            lock (set)
            {
                return set.ToImmutableHashSet();
            }
        }
    }

    public IImmutableSet<DataSet<T>> Subsets
    {
        get
        {
            lock (subsets)
            {
                return subsets.ToImmutableHashSet();
            }
        }
    }

    public DataSet(string name)
    {
        Name = name;
        set = [];
        subsets = [];
    }

    public DataSet(string name, IReadOnlyCollection<T> data)
    {
        Name = name;
        set = [.. data];
        subsets = [];
    }

    public bool AddThreadSafe(T data)
    {
        var result = false;

        lock (set)
        {
            result = set.Add(data);
        }

        if (result)
        {
            IncreaseCount();
        }

        return result;
    }

    public bool RemoveThreadSafe(T data)
    {
        var result = false;

        lock (set)
        {
            result = set.Remove(data);
        }

        if (result)
        {
            DecreaseCount();
        }

        return result;
    }

    public void DisconnectFromParentThreadSafe()
    {
        Parent?.RemoveSubsetThreadSafe(this);
    }

    public bool AddSubsetThreadSafe(DataSet<T> subset)
    {
        var result = false;

        lock (subsets)
        {
            result = subsets.Add(subset);
        }

        if (result)
        {
            subset.Parent = this;
            IncreaseCount(subset.DataCount);
        }

        return result;
    }

    private bool RemoveSubsetThreadSafe(DataSet<T> subset)
    {
        var result = false;

        lock (subsets)
        {
            result = subsets.Remove(subset);
        }

        if (result)
        {
            subset.Parent = null;
            DecreaseCount(subset.DataCount);
        }

        return result;
    }

    private void DecreaseCount(int delta = 1)
    {
        lock (this)
        {
            DataCount -= delta;
        }

        Parent?.DecreaseCount(delta);
    }

    private void IncreaseCount(int delta = 1)
    {
        lock (this)
        {
            DataCount += delta;
        }

        Parent?.IncreaseCount(delta);
    }
}
