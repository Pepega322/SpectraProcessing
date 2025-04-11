using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Domain.Models.Spectra;

namespace SpectraProcessing.Bll.Models.ScottPlot.Spectra;

public class SimpleSpectraDataPlot : SpectraDataPlot
{
    private readonly SignalXY signalXY;

    public SimpleSpectraDataPlot(SimpleSpectraData data, Color color) : base(data)
    {
        signalXY = PlottableCreator.CreateSignalXY(
            data.Points.X,
            data.Points.Y,
            color);

        Name = data.Name;

        PreviousColor = color;
    }

    public sealed override IPlottable Plottable => signalXY;

    public sealed override string Name { get; protected set; }

    public sealed override Color PreviousColor { get; protected set; }

    public override void ChangeColor(Color color)
    {
        PreviousColor = signalXY.Color;
        signalXY.Color = color;
    }
}
