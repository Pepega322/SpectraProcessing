using System.Drawing;
using System.Globalization;
using Model.Command;
using Model.Visual;

namespace Model.Data.Spectra;
internal class ESPSpectra : Spectra
{
    public string Name { get; private set; }
    private readonly string _expCfg;
    private readonly string _procCfg;

    public ESPSpectra(FileInfo file)
    {
        Name = file.Name;
        string[] contents = File.ReadAllLines(file.FullName);
        _expCfg = contents[0];
        _procCfg = contents[1];
        ExtractPoints(contents);
    }

    private ESPSpectra(ESPSpectra reference) : base(reference._points.ToList())
    {
        Name = reference.Name + "(Copy)";
        _expCfg = reference._expCfg;
        _procCfg = reference._procCfg;
    }

    public void ExtractPoints(string[] contents)
    {
        int startLineIndex = 2;
        for (int i = startLineIndex; i < contents.Length; i++)
        {
            float[] pointData = contents[i]
                .Split(' ')
                .Select(num => float.Parse(num, CultureInfo.InvariantCulture))
                .ToArray();
            _points.Add(new PointF(pointData[0], pointData[1]));
        }
    }

    public override IData CreateCopy() => new ESPSpectra(this);
    public override IEnumerable<string> ToContents() => _points.Select(p => $"{p.X} {p.Y}");
    public override void Edit(IDataEditCommand command) => throw new NotImplementedException();
    public override IData GetInfo(IGetDataInfoCommand command) => throw new NotImplementedException();
    public override IVisual Visualize() => throw new NotImplementedException();
}
