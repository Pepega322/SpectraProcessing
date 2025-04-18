using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface IDataProvider<TData>
    where TData : IWriteableData
{
    Task<TData> ReadDataAsync(string fullName);

    Task<DataSet<TData>> ReadFolderAsync(string fullName);

    Task<DataSet<TData>> ReadFolderFullDepthAsync(string fullName);

    Task DataWriteAs(TData data, string fullName);

    Task DataWriteAs(IReadOnlyCollection<TData> dataSet, string path, string extension);

    Task SetWriteAs(DataSet<TData> root, string path, string extension);
}
