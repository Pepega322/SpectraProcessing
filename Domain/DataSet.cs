using System.Collections;

namespace Domain;

public class DataSet<TData> : IEnumerable<TData>
{
	private readonly HashSet<TData> set;
	private readonly HashSet<DataSet<TData>> subsets;
	public string Name { get; protected set; }
	private int DataCount { get; set; }
	private DataSet<TData>? Parent { get; set; }
	public IEnumerable<DataSet<TData>> Subsets => subsets;

	public DataSet(string name)
	{
		Name = name;
		set = [];
		subsets = [];
	}

	public DataSet(string name, IEnumerable<TData> data)
	{
		Name = name;
		set = [..data];
		subsets = [];
	}

	public bool AddThreadSafe(TData data)
	{
		bool result;
		lock (set) result = set.Add(data);
		if (result)
			IncreaseCount();
		return result;
	}

	public bool RemoveThreadSafe(TData data)
	{
		bool result;
		lock (set) result = set.Remove(data);
		if (result) DecreaseCount();
		return result;
	}

	public void DisconnectFromParentThreadSafe()
	{
		Parent?.RemoveSubsetThreadSafe(this);
	}

	public void ReconnectToParentThreadSafe(DataSet<TData> parent)
	{
		if (Parent is not null) DisconnectFromParentThreadSafe();
		parent.AddSubsetThreadSafe(this);
	}

	public bool AddSubsetThreadSafe(DataSet<TData> subset)
	{
		bool result;
		lock (subsets) result = subsets.Add(subset);
		if (result)
		{
			subset.Parent = this;
			IncreaseCount(subset.DataCount);
		}

		return result;
	}

	private void RemoveSubsetThreadSafe(DataSet<TData> subset)
	{
		bool result;
		lock (subsets) result = subsets.Remove(subset);
		if (result)
		{
			subset.Parent = null;
			DecreaseCount(subset.DataCount);
		}
	}

	public bool Contains(TData data) => set.Contains(data);

	public Dictionary<DataSet<TData>, DataSet<TData>> CopyBranchStructureThreadSafe(string rootName)
	{
		lock (this)
		{
			var refToCopy = new Dictionary<DataSet<TData>, DataSet<TData>>
			{
				{this, new DataSet<TData>(rootName)}
			};
			var queue = new Queue<DataSet<TData>>();
			queue.Enqueue(this);
			while (queue.Count > 0)
			{
				var reference = queue.Dequeue();
				var copy = refToCopy[reference];
				foreach (var subsetInRef in reference.Subsets)
				{
					var subsetInCopy = new DataSet<TData>(subsetInRef.Name);
					copy.AddSubsetThreadSafe(subsetInCopy);
					refToCopy.Add(subsetInRef, subsetInCopy);
					queue.Enqueue(subsetInRef);
				}
			}

			return refToCopy;
		}
	}

	private void DecreaseCount(int delta = 1)
	{
		lock (this) DataCount -= delta;
		Parent?.DecreaseCount(delta);
	}

	private void IncreaseCount(int delta = 1)
	{
		lock (this) DataCount += delta;
		Parent?.IncreaseCount(delta);
	}

	public IEnumerator<TData> GetEnumerator()
	{
		return ((IEnumerable<TData>) set).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable) set).GetEnumerator();
	}
}