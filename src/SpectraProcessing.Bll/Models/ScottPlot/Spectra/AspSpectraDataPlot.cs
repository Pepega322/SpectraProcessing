using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Domain.Models.Spectra;

namespace SpectraProcessing.Bll.Models.ScottPlot.Spectra;

public class AspSpectraDataPlot : SpectraDataPlot
{
    private readonly Signal signal;

    public AspSpectraDataPlot(AspSpectraData asp, Color color) : base(asp)
    {
        signal = PlottableCreator.CreateSignal(
            asp.Points.Y,
            asp.Info.Delta,
            color);

        Name = asp.Name;

        PreviousColor = color;
    }

    public sealed override IPlottable Plottable => signal;

    public sealed override string Name { get; protected set; }

    public sealed override Color PreviousColor { get; protected set; }

    public override void ChangeColor(Color color)
    {
        PreviousColor = signal.Color;
        signal.Color = color;
    }
}
