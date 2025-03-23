using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Domain.Models.Spectra;

namespace SpectraProcessing.Bll.Models.ScottPlot.Spectra;

public class EstimatedSpectraDataPlot(EstimatedSpectraData spectraData, SignalXY plot)
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
