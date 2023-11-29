using System.Drawing;
using System.Globalization;
using Model.Command;
using Model.Visual;

namespace Model.Data.Spectra;
internal class ASPSpectra : Spectra
{
    public string Name { get; private set; }
    private readonly int _pointCount;
    private readonly float _startWavenumber;
    //private readonly float _endWavenumber;
    private readonly float _delta;

    public ASPSpectra(FileInfo file)
    {
        Name = file.Name;
        string[] contents = File.ReadAllLines(file.FullName);
        _pointCount = int.Parse(contents[0]);
        _startWavenumber = float.Parse(contents[1], CultureInfo.InvariantCulture);
        //_endWavenumber = float.Parse(text[2], CultureInfo.InvariantCulture);
        _delta = float.Parse(contents[5], CultureInfo.InvariantCulture);
        ExtractPoints(contents);
    }

    private ASPSpectra(ASPSpectra spectra) : base(spectra._points.ToList())
    {
        Name = spectra.Name + "(Copy)";
        _pointCount = spectra._pointCount;
        _startWavenumber = spectra._startWavenumber;
        //_endWavenumber = spectra._endWavenumber;
        _delta = spectra._delta;
    }

    private void ExtractPoints(string[] contents)
    {
        int startLineIndex = 7;
        float wavenumber = _startWavenumber;
        for (int i = 0; i < _pointCount; i++)
        {
            float readWavenumber = wavenumber / (float)(2 * Math.PI);
            float intensity = float.Parse(contents[startLineIndex + 1], CultureInfo.InvariantCulture);
            _points.Add(new PointF(readWavenumber, intensity));
            wavenumber += _delta;
        }
    }

    public override IData CreateCopy() => new ASPSpectra(this);
    public override IEnumerable<string> ToContents() => _points.Select(p => $"{p.X} {p.Y}");
    public override void Edit(IDataEditCommand command) => throw new NotImplementedException();
    public override IData GetInfo(IGetDataInfoCommand command) => throw new NotImplementedException();
    public override IVisual Visualize() => throw new NotImplementedException();
}
