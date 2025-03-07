namespace SpectraProcessing.Domain.InputOutput;

public interface IDataWriter
{
    Task WriteData(IWriteableData data, string path);
}
