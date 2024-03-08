using Domain.SpectraData;
using ScottPlot;
using ScottPlot.Plottables;

namespace Scott.GraphicsData;
internal class PlottableESP : PlottableSpectra {
    public PlottableESP(ESP spectra) : base(spectra) {
        lock (builder)
            Plottable = builder.Add.SignalXY(spectra.Points.X.ToArray(), spectra.Points.Y.ToArray());
    }

    public override IEnumerable<IPlottable> GetPlots() {
        yield return Plottable;
    }

    public override void SetColor(Color color) {
        Color = color;
        ((SignalXY)Plottable).Color = color;
    }
}