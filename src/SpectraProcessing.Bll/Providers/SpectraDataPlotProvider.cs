using ScottPlot;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Models.Spectra;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;
using PlotArea = ScottPlot.Plot;

namespace SpectraProcessing.Bll.Providers;

internal sealed class SpectraDataPlotProvider(
    PlotArea plotForm,
    IPalette palette
) : IDataPlotProvider<SpectraData, SpectraDataPlot>
{
    private static int _counter;

    private readonly PlotArea builder = new();

    private readonly ISet<SpectraDataPlot> plotted = new HashSet<SpectraDataPlot>();

    public Task<SpectraDataPlot> GetPlot(SpectraData plottableData)
    {
        var color = palette.GetColor(Interlocked.Increment(ref _counter));

        SpectraDataPlot result;

        switch (plottableData)
        {
            case AspSpectraData asp:
            {
                var aspPlot = builder.Add.Signal(
                    asp.Points.Y.ToArray(),
                    asp.Info.Delta,
                    color);

                result = new AspSpectraDataPlot(asp, aspPlot);

                break;
            }
            case EspSpectraData esp:
            {
                var espPlot = builder.Add.SignalXY(
                    esp.Points.X.ToArray(),
                    esp.Points.Y.ToArray(),
                    color);

                result = new EspSpectraDataPlot(esp, espPlot);

                break;
            }
            case EstimatedSpectraData estimated:
            {
                var estimatedPlot = builder.Add.SignalXY(
                    estimated.Points.X.ToArray(),
                    estimated.Points.Y.ToArray(),
                    color);

                result = new EstimatedSpectraDataPlot(estimated, estimatedPlot);

                break;
            }
            default: throw new NotSupportedException(plottableData.GetType().Name + " is not supported");
        }

        return Task.FromResult(result);
    }

    public Task<bool> IsDrew(SpectraDataPlot plot)
    {
        lock (plotted)
        {
            return Task.FromResult(plotted.Contains(plot));
        }
    }

    public Task Draw(SpectraDataPlot plt)
    {
        lock (plotted)
        {
            if (!plotted.Add(plt))
            {
                return Task.CompletedTask;
            }
        }

        lock (plotForm)
        {
            plotForm.Add.Plottable(plt.Plottable);
        }

        return Task.CompletedTask;
    }

    public Task Erase(SpectraDataPlot plt)
    {
        lock (plotted)
        {
            if (!plotted.Remove(plt))
            {
                return Task.CompletedTask;
            }
        }

        lock (plotForm)
        {
            plotForm.Remove(plt.Plottable);
        }

        return Task.CompletedTask;
    }

    public Task Resize()
    {
        lock (plotForm)
        {
            plotForm.Axes.AutoScaleX();
            plotForm.Axes.AutoScaleY();
        }

        return Task.CompletedTask;
    }

    public Task Clear()
    {
        lock (plotForm)
        {
            foreach (var plt in plotted)
            {
                plotForm.Remove(plt.Plottable);
            }
        }

        lock (plotted)
        {
            plotted.Clear();
        }

        return Task.CompletedTask;
    }
}
