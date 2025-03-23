using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Domain.Models.Spectra;

namespace SpectraProcessing.Bll.Models.ScottPlot.Spectra;

public class AspSpectraDataPlot : SpectraDataPlot
{
    private readonly Signal signal;

    public AspSpectraDataPlot(AspSpectraData asp) : base(asp)
    {
        using var builder = new Plot();

        signal = builder.Add.Signal(
            asp.Points.Y.ToArray(),
            asp.Info.Delta);

        Name = asp.Name;

        PreviousColor = signal.Color;

        Plottable = signal;
    }

    public sealed override IPlottable Plottable { get; protected set; }

    public sealed override string Name { get; protected set; }

    public sealed override Color PreviousColor { get; protected set; }

    public override void ChangeColor(Color color)
    {
        PreviousColor = signal.Color;
        signal.Color = color;
    }
}
