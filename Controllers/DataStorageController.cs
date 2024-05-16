using Controllers.Interfaces;
using Controllers.Settings;
using Domain.Storage;
using Microsoft.Extensions.Options;

namespace Controllers;

public sealed class DataStorageController<TData>(IOptions<DataStorageSettings> settings) : IDataStorageController<TData>
{
	private readonly DataStorage<TData> storage = new(settings.Value.DefaultDataSetName);
	public IEnumerable<KeyValuePair<string, DataSet<TData>>> StorageRecords => storage;

	public bool AddDataToDefaultSet(TData data)
	{
		return storage.DefaultSet.AddThreadSafe(data);
	}

	public void AddDataSet(DataSet<TData> set)
	{
		storage.Add(set.Name, set);
	}

	public void Clear()
	{
		storage.ClearThreadSafe();
	}

	public bool DeleteData(DataSet<TData> dataOwner, TData data)
	{
		return dataOwner.RemoveThreadSafe(data);
	}

	public void DeleteDataSet(DataSet<TData> set)
	{
		if (storage.ContainsKey(set.Name) && storage[set.Name] == set)
		{
			storage.RemoveThreadSafe(set.Name);
		}
		else
		{
			set.DisconnectFromParentThreadSafe();
		}
	}
}