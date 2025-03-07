namespace SpectraProcessing.Domain.InputOutput;

public interface IDataReader<TData>
{
    Task<TData> Get(string path);
}
