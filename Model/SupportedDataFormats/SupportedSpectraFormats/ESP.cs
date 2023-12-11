using System.Globalization;
using Model.SupportedCommands.DataEdit.Base;
using Model.SupportedCommands.GetData.Base;
using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.SupportedSpectraFormats.Base;

namespace Model.SupportedDataFormats.SupportedSpectraFormats;
internal class ESP : Spectra
{
    private static Dictionary<int, double[]> s_xSPlots = [];

    public const int FirstPointLineIndex = 2;

    private readonly string _expCfg;
    private readonly string _procCfg;

    internal ESP(string name, string[] contents)
    {
        Name = name;
        _expCfg = contents[0];
        _procCfg = contents[1];
        _xS = GetXS(contents);
        _yS = GetYS(contents);
    }

    private ESP(ESP reference) : base(reference)
    {
        Name = reference.Name + "(Copy)";
        _expCfg = reference._expCfg;
        _procCfg = reference._procCfg;
    }

    public override double[] GetXS(params string[] contents)
    {
        var firstHundredXSum = contents
            .Skip(FirstPointLineIndex)
            .Take(100)
            .Select(s => double.Parse(s.Split(' ')[0], CultureInfo.InvariantCulture))
            .Sum();
        var hash = HashCode.Combine(contents.Length - FirstPointLineIndex, firstHundredXSum);
        lock (s_xSPlots)
        {
            if (!s_xSPlots.ContainsKey(hash))
            {
                var xS = new double[contents.Length - FirstPointLineIndex];
                for (var i = 0; i < xS.Length; i++)
                    xS[i] = double.Parse(contents[FirstPointLineIndex + i].Split(' ')[0], CultureInfo.InvariantCulture);
                s_xSPlots.Add(hash, xS);
            }

        }
        return s_xSPlots[hash];
    }

    public override double[] GetYS(params string[] contents)
    {
        var yS = new double[PointsCount];
        for (var i = 0; i < yS.Length; i++)
            yS[i] = double.Parse(contents[FirstPointLineIndex + i].Split(' ')[1], CultureInfo.InvariantCulture);
        return yS;
    }

    public override Data CreateCopy() => new ESP(this);

    public override IEnumerable<string> ToOriginalContents()
    {
        yield return _expCfg;
        yield return _procCfg;
        foreach (var point in ToContents())
            yield return point;
    }

    public override void Edit(DataEditCommand command) => throw new NotImplementedException();
    public override Data GetInfo(GetDataCommand command) => throw new NotImplementedException();

}
