using System.Globalization;
using Model.DataFormats.Base;
using Model.DataFormats.Spectras.Base;

namespace Model.DataFormats.SpectraFormats;
public class ASP : Spectra
{
    private static Dictionary<int, IReadOnlyList<double>> s_xSPlots = [];

    public const int FirstPointIndex = 7;

    private readonly int _pointCount;
    public readonly double StartWavenumber;
    public readonly double EndWavenumber;
    private readonly int _fourLine;
    private readonly int _fiveLine;
    public readonly double Delta;

    internal ASP(string name, string[] contents)
    {
        Name = name;
        _pointCount = int.Parse(contents[0]);
        StartWavenumber = double.Parse(contents[1], CultureInfo.InvariantCulture) / (2 * Math.PI);
        //StartWavenumber = double.Parse(contents[1], CultureInfo.InvariantCulture);
        EndWavenumber = double.Parse(contents[2], CultureInfo.InvariantCulture) / (2 * Math.PI);
        //EndWavenumber = double.Parse(contents[2], CultureInfo.InvariantCulture) ;
        _fourLine = int.Parse(contents[3], CultureInfo.InvariantCulture);
        _fiveLine = int.Parse(contents[4], CultureInfo.InvariantCulture);
        Delta = double.Parse(contents[5], CultureInfo.InvariantCulture) / (2 * Math.PI);
        //Delta = double.Parse(contents[5], CultureInfo.InvariantCulture) ;
        var (xS, yS) = ReadPoints(contents);
        _xS = xS;
        _yS = yS;
    }

    private ASP(ASP spectra) : base(spectra)
    {
        Name = spectra.Name + "(Copy)";
        _pointCount = spectra._pointCount;
        StartWavenumber = spectra.StartWavenumber;
        EndWavenumber = spectra.EndWavenumber;
        _fourLine = spectra._fourLine;
        _fiveLine = spectra._fiveLine;
        Delta = spectra.Delta;
    }

    protected override (IReadOnlyList<double> xS, double[] yS) ReadPoints(params string[] contents)
    {
        var hash = HashCode.Combine(_pointCount, StartWavenumber, Delta);
        lock (s_xSPlots)
        {
            if (!s_xSPlots.ContainsKey(hash))
            {
                var newXS = new double[_pointCount];
                var wavenumber = StartWavenumber;
                for (int i = 0; i < _pointCount; i++)
                {
                    newXS[i] = wavenumber;
                    wavenumber += Delta;
                }
                s_xSPlots.Add(hash, newXS.AsReadOnly());
            }
        }

        var xS = s_xSPlots[hash];
        var yS = contents
            .Skip(FirstPointIndex)
            .Take(xS.Count)
            .Select(p => double.Parse(p, CultureInfo.InvariantCulture))
            .ToArray();

        return (xS, yS);
    }

    public override IEnumerable<string> ToOriginalContents()
    {
        yield return _pointCount.ToString();
        yield return StartWavenumber.ToString();
        yield return EndWavenumber.ToString();
        yield return _fourLine.ToString();
        yield return _fiveLine.ToString();
        yield return Delta.ToString();
        yield return "\n";
        foreach (var point in ToContents())
            yield return point;
    }

    public override Data CreateCopy() => new ASP(this);
}
