using Microsoft.Extensions.Options;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Settings;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Collections.Keys;

namespace SpectraProcessing.Bll.Controllers;

public sealed class DataStorageController<TKey, TData>(IOptions<DataStorageSettings> settings)
    : IDataStorageController<TKey, TData>
    where TKey : INamedKey
{
    private readonly DataStorage<TKey, TData> storage = new(settings.Value.DefaultDataSetName);

    public event Action? OnChange;

    public DataSet<TData> DefaultDataSet => storage.DefaultSet;

    public IReadOnlyCollection<DataSet<TData>> StorageDataSets => storage;

    public Task AddDataToDefaultSet(TData data)
    {
        if (storage.DefaultSet.AddThreadSafe(data))
        {
            OnChange?.Invoke();
        }

        return Task.CompletedTask;
    }

    public Task AddDataSet(TKey key, DataSet<TData> set)
    {
        storage.Add(key, set);

        OnChange?.Invoke();

        return Task.CompletedTask;
    }

    public Task DeleteData(DataSet<TData> dataOwner, TData data)
    {
        if (dataOwner.RemoveThreadSafe(data))
        {
            OnChange?.Invoke();
        }

        return Task.CompletedTask;
    }

    public Task DeleteDataSet(TKey key, DataSet<TData> set)
    {
        if (storage.ContainsKey(key) && storage[key] == set)
        {
            storage.RemoveThreadSafe(key);
        }
        else
        {
            set.DisconnectFromParentThreadSafe();
        }

        OnChange?.Invoke();

        return Task.CompletedTask;
    }

    public Task Clear()
    {
        storage.ClearThreadSafe();

        OnChange?.Invoke();

        return Task.CompletedTask;
    }
}
