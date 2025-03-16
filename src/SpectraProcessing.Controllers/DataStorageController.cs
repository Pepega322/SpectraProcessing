using Microsoft.Extensions.Options;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Controllers.Settings;
using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Controllers;

public sealed class DataStorageController<TData>(IOptions<DataStorageSettings> settings) : IDataStorageController<TData>
{
    private readonly DataStorage<TData> storage = new(settings.Value.DefaultDataSetName);

    public event Action? OnChange;

    public IReadOnlyCollection<DataSet<TData>> StorageData => storage;

    public Task AddDataToDefaultSet(TData data)
    {
        if (storage.DefaultSet.AddThreadSafe(data))
        {
            OnChange?.Invoke();
        }

        return Task.CompletedTask;
    }

    public Task AddDataSet(DataSet<TData> set)
    {
        storage.Add(set.Name, set);

        OnChange?.Invoke();

        return Task.CompletedTask;
    }

    public Task Clear()
    {
        storage.ClearThreadSafe();

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

    public Task DeleteDataSet(DataSet<TData> set)
    {
        if (storage.ContainsKey(set.Name) && storage[set.Name] == set)
        {
            storage.RemoveThreadSafe(set.Name);
        }
        else
        {
            set.DisconnectFromParentThreadSafe();
        }

        OnChange?.Invoke();

        return Task.CompletedTask;
    }
}
