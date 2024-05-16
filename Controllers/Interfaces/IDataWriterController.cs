using Domain.InputOutput;
using Domain.Storage;

namespace Controllers.Interfaces;

public interface IDataWriterController
{
	void DataWriteAs<TData>(TData data, string path) where TData : IWriteableData;
	void SetOnlyWriteAs<TData>(DataSet<TData> set, string path, string extension) where TData : IWriteableData;
	void SetFullDepthWriteAs<TData>(DataSet<TData> root, string path, string extension) where TData : IWriteableData;
}