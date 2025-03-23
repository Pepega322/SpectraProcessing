using System.Collections.Immutable;

namespace SpectraProcessing.Domain.Collections;

public class DataSet<TValue>
{
    private readonly HashSet<TValue> set;

    private readonly HashSet<DataSet<TValue>> subsets;

    private int DataCount { get; set; }

    private DataSet<TValue>? Parent { get; set; }

    public string Name { get; protected set; }

    public IImmutableSet<TValue> Data
    {
        get
        {
            lock (set)
            {
                return set.ToImmutableHashSet();
            }
        }
    }

    public IImmutableSet<DataSet<TValue>> Subsets
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

    public DataSet(string name, IReadOnlyCollection<TValue> data)
    {
        Name = name;
        set = [.. data];
        subsets = [];
    }

    public bool AddThreadSafe(TValue data)
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

    public bool RemoveThreadSafe(TValue data)
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

    public bool AddSubsetThreadSafe(DataSet<TValue> subset)
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

    private bool RemoveSubsetThreadSafe(DataSet<TValue> subset)
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
