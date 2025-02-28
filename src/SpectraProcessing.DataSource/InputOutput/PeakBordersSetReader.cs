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

    public PeakBordersSet Get(string fullName)
    {
        var file = new FileInfo(fullName);

        if (!file.Exists)
            throw new FileNotFoundException(fullName);

        if (file.Extension != extension)
            throw new FormatException($"file must have \"{extension}\"  extension");

        ICollection<PeakBorders> borders;
        try
        {
            borders = File.ReadLines(fullName)
                .Skip(firstLineIndex)
                .Select(s => s.Split(separator))
                .Select(values => new PeakBorders(
                    float.Parse(values[0]),
                    float.Parse(values[1])))
                .ToArray();
        }
        catch (Exception e)
        {
            throw new CorruptedFileException($"{fullName}{Environment.NewLine}{e.Message}");
        }

        return new PeakBordersSet(borders)
        {
            Name = file.Name,
        };
    }
}
