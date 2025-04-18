using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Domain.MathModeling;

namespace SpectraProcessing.Domain.Models.Peak;

public class PeakDataSet : IWriteableData
{
    private const string separator = ";";

    public const string FileExtension = "peaks";

    public static PeakDataSet Parse(string name, string[] contents)
    {
        var peaks = contents.Skip(1)
            .Select(
                line =>
                {
                    var lineParts = line.Split(separator);

                    return new PeakData(
                        center: float.Parse(lineParts[0]),
                        halfWidth: float.Parse(lineParts[2]),
                        amplitude: float.Parse(lineParts[3]),
                        gaussianContribution: float.Parse(lineParts[4]));
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
                $"{peak.Center:#.#####}{s}{area:#.#####}{s}{peak.HalfWidth:#.#####}{s}{peak.Amplitude:#.#####}{s}{peak.GaussianContribution:#.#####}";
        }
    }
}
