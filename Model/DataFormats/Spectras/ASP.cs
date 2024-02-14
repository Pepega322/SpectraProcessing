using System.Globalization;

namespace Model.DataFormats;
public class ASP : Spectra {
    public const int FirstPointIndex = 7;

    private static Dictionary<int, IReadOnlyList<float>> xSPlots = [];

    public readonly int pointCount;
    public readonly float StartWavenumber;
    public readonly float EndWavenumber;
    public readonly int FourLine;
    public readonly int FiveLine;
    public readonly float Delta;

    internal ASP(string name, string[] contents) {
        Name = name;
        pointCount = int.Parse(contents[0]);
        StartWavenumber = float.Parse(contents[1], CultureInfo.InvariantCulture) / (float)(2 * Math.PI);
        EndWavenumber = float.Parse(contents[2], CultureInfo.InvariantCulture) / (float)(2 * Math.PI);
        FourLine = int.Parse(contents[3], CultureInfo.InvariantCulture);
        FiveLine = int.Parse(contents[4], CultureInfo.InvariantCulture);
        Delta = float.Parse(contents[5], CultureInfo.InvariantCulture) / (float)(2 * Math.PI);
        var (x, y) = ReadPoints(contents);
        xS = x;
        yS = y;
    }

    private ASP(ASP spectra) : base(spectra) {
        Name = spectra.Name + "(Copy)";
        pointCount = spectra.pointCount;
        StartWavenumber = spectra.StartWavenumber;
        EndWavenumber = spectra.EndWavenumber;
        FourLine = spectra.FourLine;
        FiveLine = spectra.FiveLine;
        Delta = spectra.Delta;
    }

    protected override (IReadOnlyList<float> xS, float[] yS) ReadPoints(params string[] contents) {
        var hash = HashCode.Combine(pointCount, StartWavenumber, Delta);
        lock (xSPlots) {
            if (!xSPlots.ContainsKey(hash)) {
                var newXS = new float[pointCount];
                var wavenumber = StartWavenumber;
                for (int i = 0; i < pointCount; i++) {
                    newXS[i] = wavenumber;
                    wavenumber += Delta;
                }
                xSPlots.Add(hash, newXS.AsReadOnly());
            }
        }

        var xS = xSPlots[hash];
        var yS = contents
            .Skip(FirstPointIndex)
            .Take(xS.Count)
            .Select(p => float.Parse(p, CultureInfo.InvariantCulture))
            .ToArray();
        return (xS, yS);
    }

    public override IEnumerable<string> ToOriginalContents() {
        yield return pointCount.ToString();
        yield return StartWavenumber.ToString();
        yield return EndWavenumber.ToString();
        yield return FourLine.ToString();
        yield return FiveLine.ToString();
        yield return Delta.ToString();
        yield return "\n";
        foreach (var point in ToContents())
            yield return point;
    }

    public override Data CreateCopy() => new ASP(this);
}
