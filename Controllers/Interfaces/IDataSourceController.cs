using Domain.Storage;

namespace Controllers.Interfaces;

public interface IDataSourceController<TData>
{
	event Action? OnChange;
	DirectoryInfo Root { get; }
	Task<TData?> Read(string fullName);
	Task<DataSet<TData>> ReadFolderAsync();
	Task<DataSet<TData>> ReadFolderFullDepthAsync();
	bool ChangeFolder(string path);
	bool StepOutFolder();
	void RefreshView();
}