using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Domain.SpectraData.Formats;

namespace SpectraProcessing.Graphics.Formats;

public class AspSpectraPlot(AspSpectra spectra, Signal plot) : SpectraPlot(spectra)
{
    public override string Name { get; protected set; } = spectra.Name;

    public override IEnumerable<IPlottable> GetPlottables()
    {
        yield return plot;
    }

    public override void ChangeColor(Color color)
    {
        PreviousColor = plot.Color;
        plot.Color = color;
    }
}
