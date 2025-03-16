using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Controllers.Interfaces;

public interface IDataStorageController<TData>
{
    event Action? OnChange;

    IReadOnlyCollection<DataSet<TData>> StorageData { get; }

    Task AddDataToDefaultSet(TData data);

    Task AddDataSet(DataSet<TData> set);

    Task DeleteData(DataSet<TData> dataOwner, TData data);

    Task DeleteDataSet(DataSet<TData> set);

    Task Clear();
}
