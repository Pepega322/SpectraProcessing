using System.Drawing;
using System.Globalization;
using Model.SupportedCommands.DataEdit.Base;
using Model.SupportedCommands.GetData.Base;
using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.SupportedSpectraFormats.Base;

namespace Model.SupportedDataFormats.SupportedSpectraFormats;
internal class ESP : Spectra
{
    public const int FirstPointLineIndex = 2;

    private readonly string _expCfg;
    private readonly string _procCfg;

    internal ESP(string name, string[] contents)
    {
        Name = name;
        _expCfg = contents[0];
        _procCfg = contents[1];
        ExtractPoints(contents);
    }

    private ESP(ESP reference) : base(reference._points)
    {
        Name = reference.Name + "(Copy)";
        _expCfg = reference._expCfg;
        _procCfg = reference._procCfg;
    }

    private void ExtractPoints(string[] contents)
    {
        for (int i = FirstPointLineIndex; i < contents.Length; i++)
        {
            float[] pointData = contents[i]
                .Split(' ')
                .Select(num => float.Parse(num, CultureInfo.InvariantCulture))
                .ToArray();
            _points.Add(new PointF(pointData[0], pointData[1]));
        }
    }

    public override Data CreateCopy() => new ESP(this);

    public override IEnumerable<string> ToOriginalContents()
    {
        yield return _expCfg;
        yield return _procCfg;
        foreach (var p in _points)
            yield return $"{p.X} {p.Y}";
    }

    public override void Edit(DataEditCommand command) => throw new NotImplementedException();
    public override Data GetInfo(GetDataCommand command) => throw new NotImplementedException();
}
