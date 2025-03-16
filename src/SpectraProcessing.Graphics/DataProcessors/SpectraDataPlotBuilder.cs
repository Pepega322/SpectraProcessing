using ScottPlot;
using SpectraProcessing.Domain.DataProcessors;
using SpectraProcessing.Models.Spectra;
using SpectraProcessing.Models.Spectra.Abstractions;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.DataProcessors;

public class SpectraDataPlotBuilder(IPalette palette) : IDataPlotBuilder<SpectraData, SpectraDataPlot>
{
    private readonly Lock locker = new();

    private readonly PlotArea builder = new();

    private int counter;

    public Task<SpectraDataPlot> GetPlot(SpectraData plottableData)
    {
        Color color;

        lock (locker)
        {
            color = palette.GetColor(counter);
            counter++;
        }

        SpectraDataPlot result;

        switch (plottableData)
        {
            case AspSpectraData asp:
            {
                var aspPlot = builder.Add.Signal(asp.Points.Y.ToArray(), asp.Info.Delta, color);

                result = new AspSpectraDataPlot(asp, aspPlot);

                break;
            }
            case EspSpectraData esp:
            {
                var espPlot = builder.Add.SignalXY(esp.Points.X.ToArray(), esp.Points.Y.ToArray(), color);

                result = new EspSpectraDataPlot(esp, espPlot);

                break;
            }
            default: throw new NotSupportedException();
        }

        return Task.FromResult(result);
    }
}
