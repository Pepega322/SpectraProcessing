using SpectraProcessing.DataSource.Exceptions;
using SpectraProcessing.Domain.InputOutput;
using SpectraProcessing.MathStatistics.InputOutput;
using SpectraProcessing.MathStatistics.SpectraProcessing;

namespace SpectraProcessing.DataSource.InputOutput;

public class PeakBordersSetReader : IDataReader<PeakBordersSet>
{
    private const string extension = ".borders";
    private const int firstLineIndex = 1;
    private const char separator = ';';

    public async Task<PeakBordersSet> ReadData(string fullName)
    {
        var file = new FileInfo(fullName);

        if (!file.Exists)
            throw new FileNotFoundException(fullName);

        if (file.Extension != extension)
            throw new FormatException($"file must have \"{extension}\"  extension");

        IReadOnlyCollection<PeakBorders> borders;
        try
        {
            var lines = await File.ReadAllLinesAsync(fullName);
            borders = lines
                .Skip(firstLineIndex)
                .Select(s => s.Split(separator))
                .Select(
                    values => new PeakBorders(
                        float.Parse(values[0]),
                        float.Parse(values[1])))
                .ToArray();
        }
        catch (Exception e)
        {
            throw new CorruptedFileException($"{fullName}{Environment.NewLine}{e.Message}");
        }

        return new PeakBordersSet
        {
            Name = file.Name,
            Borders = borders,
        };
    }
}
