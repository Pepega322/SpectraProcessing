using System.Globalization;

namespace Model.DataFormats;
public class ASP : Spectra {
    private const int FirstPointIndex = 7;

    internal static ASP Parse(string name, string[] contents) {
        var info = new ASPInfo(contents);
        var xPoints = new List<float>(info.PointCount);
        var x = info.StartWavenumber;
        for (var i = 0; i < xPoints.Capacity; i++) {
            xPoints.Add(x);
            x += info.Delta;
        }
        var yPoints = contents
            .Skip(FirstPointIndex)
            .Select(line => float.Parse(line, CultureInfo.InvariantCulture))
            .ToList();
        var points = new SpectraPoints(xPoints, yPoints);
        return new ASP(name, points, info);
    }

    public ASPInfo Info;

    private ASP(string name, SpectraPoints points, ASPInfo info)
        : base(name, points) {
        this.Info = info;
        Format = SpectraFormat.ASP;
    }

    private ASP(ASP reference)
        : base(reference.Name, reference.Points) {
        Info = reference.Info;
        Format = SpectraFormat.ASP;
    }

    public override Data CreateCopy() => new ASP(this);
}