using System.Drawing;
using System.Globalization;
using Model.SupportedCommands.DataEdit.Base;
using Model.SupportedCommands.GetData.Base;
using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.SupportedSpectraFormats.Base;

namespace Model.SupportedDataFormats.SupportedSpectraFormats;
internal class ASP : Spectra
{
    public const int FirstPointLineIndex = 7;

    private readonly int _pointCount;
    private readonly float _startWavenumber;
    private readonly float _endWavenumber;
    private readonly int _fourLine;
    private readonly int _fiveLine;
    private readonly float _delta;

    public ASP(string name, string[] contents)
    {
        Name = name;
        _pointCount = int.Parse(contents[0]);
        _startWavenumber = float.Parse(contents[1], CultureInfo.InvariantCulture);
        _endWavenumber = float.Parse(contents[2], CultureInfo.InvariantCulture);
        _fourLine = int.Parse(contents[3], CultureInfo.InvariantCulture);
        _fiveLine = int.Parse(contents[4], CultureInfo.InvariantCulture);
        _delta = float.Parse(contents[5], CultureInfo.InvariantCulture);
        ExtractPoints(contents);
    }

    private ASP(ASP spectra) : base(spectra._points)
    {
        Name = spectra.Name + "(Copy)";
        _pointCount = spectra._pointCount;
        _startWavenumber = spectra._startWavenumber;
        _endWavenumber = spectra._endWavenumber;
        _fourLine = spectra._fourLine;
        _fiveLine = spectra._fiveLine;
        _delta = spectra._delta;
    }

    private void ExtractPoints(string[] contents)
    {
        float wavenumber = _startWavenumber;
        for (int i = 0; i < _pointCount; i++)
        {
            float readWavenumber = wavenumber / (float)(2 * Math.PI);
            float intensity = float.Parse(contents[FirstPointLineIndex + i], CultureInfo.InvariantCulture);
            _points.Add(new PointF(readWavenumber, intensity));
            wavenumber += _delta;
        }
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
        foreach (var p in _points)
            yield return p.Y.ToString();
    }

    public override Data CreateCopy() => new ASP(this);

    public override void Edit(DataEditCommand command) => throw new NotImplementedException();

    public override Data GetInfo(GetDataCommand command) => throw new NotImplementedException();
}
