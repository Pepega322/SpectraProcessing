using Microsoft.Extensions.Options;
using SpectraProcessing.Bll.Models.Settings;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Collections.Keys;

namespace SpectraProcessing.Bll.Providers;

internal sealed class DataStorageProvider<TKey, TData>(IOptions<DataStorageSettings> settings)
    : IDataStorageProvider<TKey, TData>
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
