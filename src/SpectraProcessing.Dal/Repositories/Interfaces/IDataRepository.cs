using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Dal.Repositories.Interfaces;

public interface IDataRepository<TData> where TData : IWriteableData
{
    Task<TData> ReadData(string path);

    Task WriteData(TData data, string path);
}
