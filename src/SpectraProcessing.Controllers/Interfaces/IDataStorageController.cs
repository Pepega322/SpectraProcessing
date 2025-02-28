using SpectraProcessing.Domain.Storage;

namespace SpectraProcessing.Controllers.Interfaces;

public interface IDataStorageController<TData>
{
    event Action? OnChange;
    IEnumerable<DataSet<TData>> StorageData { get; }
    bool AddDataToDefaultSet(TData data);
    void AddDataSet(DataSet<TData> set);
    bool DeleteData(DataSet<TData> dataOwner, TData data);
    void DeleteDataSet(DataSet<TData> set);
    void Clear();
}
