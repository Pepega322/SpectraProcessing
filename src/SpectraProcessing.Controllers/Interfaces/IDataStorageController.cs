using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Controllers.Interfaces;

public interface IDataStorageController<TData>
{
    event Action? OnChange;
    IReadOnlyCollection<DataSet<TData>> StorageData { get; }
    void AddDataToDefaultSet(TData data);
    void AddDataSet(DataSet<TData> set);
    void DeleteData(DataSet<TData> dataOwner, TData data);
    void DeleteDataSet(DataSet<TData> set);
    void Clear();
}
