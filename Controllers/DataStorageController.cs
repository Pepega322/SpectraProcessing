using Domain;

namespace Controllers;

public class DataStorageController<TData>(string defaultSetKey)
{
	private readonly DataStorage<TData> storage = new(defaultSetKey);
	public IEnumerable<KeyValuePair<string, DataSet<TData>>> StorageRecords => storage;

	public bool AddDataToDefault(TData data)
	{
		return storage.DefaultSet.AddThreadSafe(data);
	}

	public void AddSet(DataSet<TData> set)
	{
		storage.AddThreadSafe(set.Name, set);
	}

	public void Clear()
	{
		storage.ClearThreadSafe();
	}

	public bool DeleteData(DataSet<TData> dataOwner, TData data)
	{
		return dataOwner.RemoveThreadSafe(data);
	}

	public void DeleteSet(DataSet<TData> set)
	{
		if (storage.ContainsKeyThreadSafe(set.Name) && storage[set.Name] == set)
		{
			storage.RemoveThreadSafe(set.Name);
		}
		else
		{
			set.DisconnectFromParentThreadSafe();
		}
	}
}