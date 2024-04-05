using Domain.SpectraData;
using Domain.SpectraData.Support;
using Scott.GraphicsData;
using System.Globalization;

namespace Scott.Data;

internal class ScottAsp(string name, SpectraPoints points, ASPInfo info) : Asp(name, points, info) {
	public static ScottAsp Parse(string name, string[] contents) {
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
		return new ScottAsp(name, points, info);
	}

	private SpectraPlot? plottable;

	public override SpectraPlot GetPlot() {
		return plottable ??= new PlottableASP(this);
	}

	public override Spectra Copy() => new ScottAsp(Name, Points, Info);
}