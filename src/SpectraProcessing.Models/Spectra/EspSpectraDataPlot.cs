using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Models.Spectra;

public class EspSpectraDataPlot(EspSpectraData spectraData, SignalXY plot)
    : SpectraDataPlot(spectraData, plot)
{
    public override string Name { get; protected set; } = spectraData.Name;

    public override Color PreviousColor { get; protected set; }

    public override void ChangeColor(Color color)
    {
        PreviousColor = plot.Color;
        plot.Color = color;
    }
}
