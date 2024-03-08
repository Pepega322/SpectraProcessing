using Domain.SpectraData;
using Domain.SpectraData.Support;
using Scott.GraphicsData;
using System.Globalization;

namespace Scott.Data;
internal class ScottASP(string name, SpectraPoints points, ASPInfo info) : ASP(name, points, info) {
    public static ScottASP Parse(string name, string[] contents) {
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
        return new ScottASP(name, points, info);
    }

    private SpectraPlot? plottable;

    public override SpectraPlot GetPlot() {
        if (plottable == null)
            plottable = new PlottableASP(this);
        return plottable;
    }

    public override Spectra Copy() => new ScottASP(Name, Points, Info);
}
