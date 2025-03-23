using SpectraProcessing.Domain.Collections;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface IDataProvider<TData>
{
    Task<DataSet<TData>> ReadFolderAsync(string fullName);

    Task<DataSet<TData>> ReadFolderFullDepthAsync(string fullName);

    Task DataWriteAs(TData data, string path);

    Task SetOnlyWriteAs(DataSet<TData> set, string path, string extension);

    Task SetFullDepthWriteAs(DataSet<TData> root, string path, string extension);
}
