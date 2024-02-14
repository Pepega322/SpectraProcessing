using System.Globalization;

namespace Model.DataFormats;
public class ESP : Spectra {
    public const int FirstPointIndex = 2;

    private static Dictionary<int, IReadOnlyList<float>> xSPlots = [];

    private readonly string expCfg;
    private readonly string procCfg;

    public ESP(string name, string[] contents) {
        Name = name;
        expCfg = contents[0];
        procCfg = contents[1];
        var (x, y) = ReadPoints(contents);
        xS = x;
        yS = y;
    }

    private ESP(ESP reference) : base(reference) {
        Name = reference.Name + "(Copy)";
        expCfg = reference.expCfg;
        procCfg = reference.procCfg;
    }

    protected override (IReadOnlyList<float> xS, float[] yS) ReadPoints(params string[] contents) {
        var pairs = contents
            .Skip(FirstPointIndex)
            .Select(x => x.Split(' '));
        var first5Sum = pairs
            .Take(5)
            .Select(pair => float.Parse(pair[0], CultureInfo.InvariantCulture))
            .Sum();
        var hash = HashCode.Combine(contents.Length - FirstPointIndex, first5Sum);
        lock (xSPlots) {
            if (!xSPlots.ContainsKey(hash)) {
                var newXS = pairs
                    .Select(p => float.Parse(p[0], CultureInfo.InvariantCulture))
                    .ToArray();
                xSPlots.Add(hash, newXS);
            }
        }

        var xS = xSPlots[hash];
        var yS = pairs
            .Take(xS.Count)
            .Select(pair => float.Parse(pair[1], CultureInfo.InvariantCulture))
            .ToArray();
        return (xS, yS);
    }

    public override Data CreateCopy() => new ESP(this);

    public override IEnumerable<string> ToOriginalContents() {
        yield return expCfg;
        yield return procCfg;
        foreach (var point in ToContents())
            yield return point;
    }
}
