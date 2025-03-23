using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Collections.Keys;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface IDataStorageProvider<in TKey, TData>
    where TKey : INamedKey
{
    event Action? OnChange;

    DataSet<TData> DefaultDataSet { get; }

    IReadOnlyCollection<DataSet<TData>> StorageDataSets { get; }

    Task AddDataToDefaultSet(TData data);

    Task AddDataSet(TKey key, DataSet<TData> set);

    Task DeleteData(DataSet<TData> dataOwner, TData data);

    Task DeleteDataSet(TKey key, DataSet<TData> set);

    Task Clear();
}
