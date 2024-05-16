using System.Collections;

namespace Domain.Storage;

public class DataSet<T> : IEnumerable<T>
{
	public string Name { get; protected set; }
	public int DataCount { get; private set; }
	public IEnumerable<DataSet<T>> Subsets => subsets;
	private readonly HashSet<T> set;
	private readonly HashSet<DataSet<T>> subsets;
	private DataSet<T>? Parent { get; set; }

	public DataSet(string name)
	{
		Name = name;
		set = [];
		subsets = [];
	}

	public DataSet(string name, IEnumerable<T> data)
	{
		Name = name;
		set = [..data];
		subsets = [];
	}

	public bool AddThreadSafe(T data)
	{
		bool result;
		lock (set) result = set.Add(data);
		if (result)
			IncreaseCount();
		return result;
	}

	public bool RemoveThreadSafe(T data)
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

	public void ReconnectToParentThreadSafe(DataSet<T> parent)
	{
		if (Parent is not null)
		{
			DisconnectFromParentThreadSafe();
		}

		parent.AddSubsetThreadSafe(this);
	}

	public bool AddSubsetThreadSafe(DataSet<T> subset)
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

	public bool RemoveSubsetThreadSafe(DataSet<T> subset)
	{
		bool result;
		lock (subsets) result = subsets.Remove(subset);
		if (result)
		{
			subset.Parent = null;
			DecreaseCount(subset.DataCount);
		}

		return result;
	}

	public bool Contains(T data) => set.Contains(data);

	public Dictionary<DataSet<T>, DataSet<T>> CopyBranchStructureThreadSafe(string rootName)
	{
		lock (this)
		{
			var refToCopy = new Dictionary<DataSet<T>, DataSet<T>>
			{
				{this, new DataSet<T>(rootName)}
			};
			var queue = new Queue<DataSet<T>>();
			queue.Enqueue(this);
			while (queue.Count > 0)
			{
				var reference = queue.Dequeue();
				var copy = refToCopy[reference];
				foreach (var subsetInRef in reference.Subsets)
				{
					var subsetInCopy = new DataSet<T>(subsetInRef.Name);
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

	public IEnumerator<T> GetEnumerator()
	{
		return ((IEnumerable<T>) set).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable) set).GetEnumerator();
	}
}