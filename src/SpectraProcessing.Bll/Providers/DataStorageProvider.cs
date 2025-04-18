using System.Collections.Concurrent;
using System.Collections.Immutable;
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
    private readonly ConcurrentDictionary<TKey, DataSet<TData>> storage = [];

    public event Action? OnChange;

    public DataSet<TData> DefaultSet { get; private set; } = new(settings.Value.DefaultDataSetName);

    public IImmutableDictionary<TKey, DataSet<TData>> Sets => storage.ToImmutableDictionary();

    public Task AddDataToDefaultSet(TData data)
    {
        if (DefaultSet.AddThreadSafe(data))
        {
            OnChange?.Invoke();
        }

        return Task.CompletedTask;
    }

    public Task AddDataSet(TKey key, DataSet<TData> set)
    {
        storage.TryAdd(key, set);

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

    public Task DeleteDataSet(TKey key)
    {
        if (key.Name == settings.Value.DefaultDataSetName)
        {
            DefaultSet = new DataSet<TData>(settings.Value.DefaultDataSetName);
        }
        else
        {
            storage.TryRemove(key, out _);
        }

        OnChange?.Invoke();

        return Task.CompletedTask;
    }

    public Task DeleteDataSet(DataSet<TData> set)
    {
        var key = storage.FirstOrDefault(x => x.Value == set).Key;

        if (key is null)
        {
            set.DisconnectFromParentThreadSafe();
        }
        else
        {
            storage.TryRemove(key, out _);
        }

        OnChange?.Invoke();

        return Task.CompletedTask;
    }

    public Task Clear()
    {
        DefaultSet = new DataSet<TData>(settings.Value.DefaultDataSetName);

        storage.Clear();

        OnChange?.Invoke();

        return Task.CompletedTask;
    }
}
