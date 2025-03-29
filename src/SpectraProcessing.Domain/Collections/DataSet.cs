namespace SpectraProcessing.Domain.Collections;

public class DataSet<TValue>
{
    private readonly ISet<TValue> data;

    private readonly ISet<DataSet<TValue>> subsets;

    private int DataCount { get; set; }

    private DataSet<TValue>? Parent { get; set; }

    public string Name { get; protected set; }

    public IReadOnlyList<TValue> Data
    {
        get
        {
            lock (data)
            {
                return data.ToArray();
            }
        }
    }

    public IReadOnlyList<DataSet<TValue>> Subsets
    {
        get
        {
            lock (subsets)
            {
                return subsets.ToArray();
            }
        }
    }

    public DataSet(string name)
    {
        Name = name;
        data = new HashSet<TValue>();
        subsets = new HashSet<DataSet<TValue>>();
    }

    public DataSet(string name, IReadOnlyCollection<TValue> data)
    {
        Name = name;
        this.data = new HashSet<TValue>(data);
        subsets = new HashSet<DataSet<TValue>>();
    }

    public bool AddThreadSafe(TValue value)
    {
        bool result;

        lock (data)
        {
            result = data.Add(value);
        }

        if (result)
        {
            IncreaseCount();
        }

        return result;
    }

    public bool RemoveThreadSafe(TValue value)
    {
        bool result;

        lock (data)
        {
            result = data.Remove(value);
        }

        if (result)
        {
            DecreaseCount();
        }

        return result;
    }

    public void ClearThreadSafe()
    {
        lock (data)
        {
            DecreaseCount(data.Count);
            data.Clear();
        }
    }

    public void DisconnectFromParentThreadSafe()
    {
        Parent?.RemoveSubsetThreadSafe(this);
    }

    public bool AddSubsetThreadSafe(DataSet<TValue> subset)
    {
        bool result;

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
        bool result;

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
