namespace SpectraProcessing.Domain.InputOutput;

public interface IDataWriter
{
    void WriteData(IWriteableData data, string path);
}
