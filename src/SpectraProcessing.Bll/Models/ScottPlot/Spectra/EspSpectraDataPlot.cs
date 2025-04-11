using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Domain.Models.Spectra;

namespace SpectraProcessing.Bll.Models.ScottPlot.Spectra;

public class EspSpectraDataPlot : SpectraDataPlot
{
    private readonly SignalXY signalXY;

    public EspSpectraDataPlot(EspSpectraData esp, Color color) : base(esp)
    {
        signalXY = PlottableCreator.CreateSignalXY(
            esp.Points.X,
            esp.Points.Y,
            color);

        Name = esp.Name;

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
