using SpectraProcessing.DataSource.Exceptions;
using SpectraProcessing.Domain.InputOutput;
using SpectraProcessing.Domain.SpectraData;
using SpectraProcessing.Domain.SpectraData.Formats;
using SpectraProcessing.Domain.SpectraData.Parser;

namespace SpectraProcessing.DataSource.InputOutput;

public class SpectraFileReader(ISpectraParser parser) : IDataReader<Spectra>
{
    public async Task<Spectra> ReadData(string fullName)
    {
        var file = new FileInfo(fullName);

        if (!file.Exists)
            throw new FileNotFoundException(fullName);

        if (!Enum.TryParse(file.Extension.TrimStart('.'), true, out SpectraFormat format))
            throw new UndefinedFileException(fullName);

        var lines = await File.ReadAllLinesAsync(file.FullName);

        try
        {
            return parser.Parse(format, file.Name, lines);
        }
        catch (Exception e)
        {
            throw new CorruptedFileException($"{fullName}{Environment.NewLine}{e.Message}");
        }
    }
}
