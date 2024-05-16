using Domain.Storage;

namespace Controllers.Interfaces;

public interface IDataStorageController<TData>
{
	IEnumerable<KeyValuePair<string, DataSet<TData>>> StorageRecords { get; }
	bool AddDataToDefaultSet(TData data);
	void AddDataSet(DataSet<TData> set);
	bool DeleteData(DataSet<TData> dataOwner, TData data);
	void DeleteDataSet(DataSet<TData> set);
	void Clear();
}