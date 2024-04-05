﻿using Domain.SpectraData;
using Domain.SpectraData.Support;
using Scott.GraphicsData;
using System.Globalization;

namespace Scott.Data;
internal class ScottEsp(string name, SpectraPoints points, ESPInfo info) : Esp(name, points, info) {
    public static Esp Parse(string name, string[] contents) {
        var info = new ESPInfo(contents);
        var pointCount = contents.Length - FirstPointIndex;
        var xPoints = new List<float>(pointCount);
        var yPoints = new List<float>(pointCount);
        foreach (var pair in contents.Skip(FirstPointIndex).Select(line => line.Split(' '))) {
            xPoints.Add(float.Parse(pair[0], CultureInfo.InvariantCulture));
            yPoints.Add(float.Parse(pair[1], CultureInfo.InvariantCulture));
        }
        var points = new SpectraPoints(xPoints, yPoints);
        return new ScottEsp(name, points, info);
    }

    private SpectraPlot? plottable;

    public override SpectraPlot GetPlot() {
        plottable ??= new PlottableESP(this);
        return plottable;
    }

    public override Spectra Copy() => new ScottEsp(Name, Points, Info);
}
