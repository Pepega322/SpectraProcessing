using System.Globalization;
using Model.DataFormats.Base;
using Model.DataFormats.Spectras.Base;

namespace Model.DataFormats.SpectraFormats;
internal class ESP : Spectra
{
    private static Dictionary<int, IReadOnlyList<double>> s_xSPlots = [];

    public const int FirstPointIndex = 2;

    private readonly string _expCfg;
    private readonly string _procCfg;

    public ESP(string name, string[] contents)
    {
        Name = name;
        _expCfg = contents[0];
        _procCfg = contents[1];
        var (xS, yS) = ReadPoints(contents);
        _xS = xS;
        _yS = yS;
    }

    private ESP(ESP reference) : base(reference)
    {
        Name = reference.Name + "(Copy)";
        _expCfg = reference._expCfg;
        _procCfg = reference._procCfg;
    }

    protected override (IReadOnlyList<double> xS, double[] yS) ReadPoints(params string[] contents)
    {
        var pairs = contents
            .Skip(FirstPointIndex)
            .Select(x => x.Split(' '));
        var first5Sum = pairs
            .Take(5)
            .Select(pair => double.Parse(pair[0], CultureInfo.InvariantCulture))
            .Sum();
        var hash = HashCode.Combine(contents.Length - FirstPointIndex, first5Sum);
        lock (s_xSPlots)
        {
            if (!s_xSPlots.ContainsKey(hash))
            {
                var newXS = pairs
                    .Select(p => double.Parse(p[0], CultureInfo.InvariantCulture))
                    .ToArray();
                s_xSPlots.Add(hash, newXS);
            }
        }

        var xS = s_xSPlots[hash];
        var yS = pairs
            .Take(xS.Count)
            .Select(pair => double.Parse(pair[1], CultureInfo.InvariantCulture))
            .ToArray();
        return (xS, yS);
    }

    public override Data CreateCopy() => new ESP(this);

    public override IEnumerable<string> ToOriginalContents()
    {
        yield return _expCfg;
        yield return _procCfg;
        foreach (var point in ToContents())
            yield return point;
    }
}
