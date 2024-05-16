using Domain.Storage;
using Controllers.Interfaces;
using Controllers.Settings;
using Microsoft.Extensions.Options;

namespace Controllers;

public sealed class DataStorageController<TData>(IOptions<DataStorageSettings> settings) : IDataStorageController<TData>
{
	private readonly DataStorage<TData> storage = new(settings.Value.DefaultDataSetName);
	public event Action? OnChange;
	public IEnumerable<DataSet<TData>> StorageData => storage;

	public bool AddDataToDefaultSet(TData data)
	{
		var result = storage.DefaultSet.AddThreadSafe(data);
		if (result) OnChange?.Invoke();
		return result;
	}

	public void AddDataSet(DataSet<TData> set)
	{
		storage.Add(set.Name, set);
		OnChange?.Invoke();
	}

	public void Clear()
	{
		storage.ClearThreadSafe();
		OnChange?.Invoke();
	}

	public bool DeleteData(DataSet<TData> dataOwner, TData data)
	{
		var result = dataOwner.RemoveThreadSafe(data);
		if (result) OnChange?.Invoke();
		return result;
	}

	public void DeleteDataSet(DataSet<TData> set)
	{
		if (storage.ContainsKey(set.Name) && storage[set.Name] == set)
			storage.RemoveThreadSafe(set.Name);
		else
			set.DisconnectFromParentThreadSafe();
		OnChange?.Invoke();
	}
}