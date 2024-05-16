using Domain.Storage;

namespace Controllers.Interfaces;

public interface IDataReaderController<TData>
{
	DirectoryInfo Root { get; }
	TData? Read(string fullName);
	DataSet<TData> ReadRoot();
	DataSet<TData> ReadRootFullDepth();
	bool ChangeRoot(string path);
	bool StepBack();
}