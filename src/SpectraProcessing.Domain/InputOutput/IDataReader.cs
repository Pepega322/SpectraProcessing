namespace SpectraProcessing.Domain.InputOutput;

public interface IDataReader<TData>
{
    Task<TData> ReadData(string path);
}
