using SpectraProcessing.Domain.Storage;

namespace SpectraProcessing.Controllers.Interfaces;

public interface IDataSourceController<TData>
{
    Task<DataSet<TData>> ReadFolderAsync(string fullName);
    Task<DataSet<TData>> ReadFolderFullDepthAsync(string fullName);
}
