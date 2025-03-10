using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Domain.SpectraData.Formats;

namespace SpectraProcessing.Graphics.Formats;

public class EspSpectraPlot(EspSpectra spectra, SignalXY plot) : SpectraPlot(spectra)
{
    public override string Name { get; protected set; } = spectra.Name;

    public override IReadOnlyCollection<IPlottable> Plottables => [plot];

    public override void ChangeColor(Color color)
    {
        PreviousColor = plot.Color;
        plot.Color = color;
    }
}
