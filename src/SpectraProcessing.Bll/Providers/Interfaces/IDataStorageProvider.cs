using System.Collections.Immutable;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Collections.Keys;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface IDataStorageProvider<TKey, TData>
    where TKey : INamedKey
{
    event Action? OnChange;

    DataSet<TData> DefaultSet { get; }

    IImmutableDictionary<TKey, DataSet<TData>> Sets { get; }

    Task AddDataToDefaultSet(TData data);

    Task AddDataSet(TKey key, DataSet<TData> set);

    Task DeleteData(DataSet<TData> dataOwner, TData data);

    Task DeleteDataSet(TKey key);

    Task DeleteDataSet(DataSet<TData> set);

    Task Clear();
}
