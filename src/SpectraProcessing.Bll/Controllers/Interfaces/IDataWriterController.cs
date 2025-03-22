using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface IDataWriterController
{
    Task DataWriteAs<TData>(TData data, string path) where TData : IWriteableData;

    Task SetOnlyWriteAs<TData>(DataSet<TData> set, string path, string extension) where TData : IWriteableData;

    Task SetFullDepthWriteAs<TData>(DataSet<TData> root, string path, string extension) where TData : IWriteableData;
}
