using System.Globalization;
using Model.SupportedCommands.DataEdit.Base;
using Model.SupportedCommands.GetData.Base;
using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.SupportedSpectraFormats.Base;

namespace Model.SupportedDataFormats.SupportedSpectraFormats;
internal class ASP : Spectra
{
    private static Dictionary<int, double[]> s_xSPlots = [];

    public const int FirstPointLineIndex = 7;

    private readonly int _pointCount;
    private readonly double _startWavenumber;
    private readonly double _endWavenumber;
    private readonly int _fourLine;
    private readonly int _fiveLine;
    private readonly double _delta;

    public ASP(string name, string[] contents)
    {
        Name = name;
        _pointCount = int.Parse(contents[0]);
        _startWavenumber = double.Parse(contents[1], CultureInfo.InvariantCulture);
        _endWavenumber = double.Parse(contents[2], CultureInfo.InvariantCulture);
        _fourLine = int.Parse(contents[3], CultureInfo.InvariantCulture);
        _fiveLine = int.Parse(contents[4], CultureInfo.InvariantCulture);
        _delta = double.Parse(contents[5], CultureInfo.InvariantCulture);
        _xS = GetXS(contents);
        _yS = GetYS(contents);
    }

    private ASP(ASP spectra) : base(spectra)
    {
        Name = spectra.Name + "(Copy)";
        _pointCount = spectra._pointCount;
        _startWavenumber = spectra._startWavenumber;
        _endWavenumber = spectra._endWavenumber;
        _fourLine = spectra._fourLine;
        _fiveLine = spectra._fiveLine;
        _delta = spectra._delta;
    }

    public override double[] GetXS(params string[] contents)
    {
        var hash = HashCode.Combine(_pointCount, _startWavenumber, _delta);
        lock (s_xSPlots)
        {
            if (!s_xSPlots.ContainsKey(hash))
            {
                var xS = new double[_pointCount];
                var wavenumber = _startWavenumber;
                for (int i = 0; i < _pointCount; i++)
                {
                    xS[i] = wavenumber / (2 * Math.PI);
                    wavenumber += _delta;
                }
                s_xSPlots.Add(hash, xS);
            }
        }
        return s_xSPlots[hash];
    }

    public override double[] GetYS(params string[] contents)
    {
        var yS = new double[PointsCount];
        for (var i = 0; i < yS.Length; ++i)
            yS[i] = double.Parse(contents[FirstPointLineIndex + i], CultureInfo.InvariantCulture);
        return yS;
    }

    public override IEnumerable<string> ToOriginalContents()
    {
        yield return _pointCount.ToString();
        yield return _startWavenumber.ToString();
        yield return _endWavenumber.ToString();
        yield return _fourLine.ToString();
        yield return _fiveLine.ToString();
        yield return _delta.ToString();
        yield return "\n";
        foreach (var point in ToContents())
            yield return point;
    }

    public override Data CreateCopy() => new ASP(this);

    public override void Edit(DataEditCommand command) => throw new NotImplementedException();

    public override Data GetInfo(GetDataCommand command) => throw new NotImplementedException();

}
