using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Domain.InputOutput;

namespace SpectraProcessing.DataSource.InputOutput;

public class FileWriter(FileMode mode) : IDataWriter
{
    private readonly FileStreamOptions options = new()
    {
        Mode = mode,
        Access = FileAccess.Write,
    };

    public async Task WriteData(IWriteableData data, string fullName)
    {
        await using var writer = new StreamWriter(fullName, options);

        var text = string.Join(Environment.NewLine, data.ToContents());

        await writer.WriteAsync(text);
    }
}
