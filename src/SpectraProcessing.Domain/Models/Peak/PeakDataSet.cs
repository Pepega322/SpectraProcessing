using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Domain.MathModeling.Peaks;

namespace SpectraProcessing.Domain.Models.Peak;

public class PeakDataSet : IWriteableData
{
    private const string separator = ";";

    public const string FileExtension = "peaks";

    public static PeakDataSet Parse(string name, string[] contents)
    {
        var peaks = contents.Skip(1)
            .Select(line =>
            {
                var lineParts = line.Split(separator);

                var center = float.Parse(lineParts[0]);
                var halfWidth = float.Parse(lineParts[2]);
                var amplitude = float.Parse(lineParts[3]);
                var gaussianContribution = float.Parse(lineParts[4]);

                return new PeakData(
                    center,
                    amplitude,
                    halfWidth,
                    gaussianContribution);
            })
            .ToArray();

        return new PeakDataSet(peaks, name);
    }

    public IReadOnlyCollection<PeakData> Peaks { get; }

    public string Name { get; }

    public string Extension => FileExtension;

    public PeakDataSet(IReadOnlyCollection<PeakData> peaks, string name)
    {
        Peaks = peaks;
        Name = name;
    }

    public IEnumerable<string> ToContents()
    {
        const string s = separator;
        const string firstLine = $"Center{s}Area{s}HalfWidth{s}Amplitude{s}GaussianContribution";

        yield return firstLine;
        foreach (var peak in Peaks.OrderBy(p => p.Center))
        {
            var area = peak.GetPeakArea();

            yield return
                $"{peak.Center:0.#####}{s}{area:0.#####}{s}{peak.HalfWidth:0.#####}{s}{peak.Amplitude:0.#####}{s}{peak.GaussianContribution:0.#####}";
        }
    }
}
