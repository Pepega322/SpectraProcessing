using SpectraProcessing.Dal.Exceptions;
using SpectraProcessing.Dal.Repositories.Interfaces;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Dal.Repositories;

internal class PeakDataIORepository : IDataRepository<PeakDataSet>
{
    private readonly FileStreamOptions options = new()
    {
        Mode = FileMode.Create,
        Access = FileAccess.Write,
    };

    public async Task<PeakDataSet> ReadData(string fullName)
    {
        var file = new FileInfo(fullName);

        if (file.Exists is false)
        {
            throw new FileNotFoundException(fullName);
        }

        if (file.Extension.TrimStart('.') != PeakDataSet.FileExtension)
        {
            throw new UndefinedFileException(fullName);
        }

        var contents = await File.ReadAllLinesAsync(file.FullName);

        try
        {
            return PeakDataSet.Parse(file.Name, contents);
        }
        catch (Exception e)
        {
            throw new CorruptedFileException($"{fullName} {Environment.NewLine} {e.Message}");
        }
    }

    public async Task WriteData(PeakDataSet data, string fullName)
    {
        await using var writer = new StreamWriter(fullName, options);

        var text = string.Join(Environment.NewLine, data.ToContents());

        await writer.WriteAsync(text);
    }
}
