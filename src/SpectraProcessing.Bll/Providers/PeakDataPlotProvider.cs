using System.Collections.Concurrent;
using ScottPlot;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.MathModeling;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Providers;

internal sealed class PeakDataPlotProvider : IPeakDataPlotProvider
{
    private static readonly ConcurrentDictionary<PeakData, PeakDataPlot> PeakDataPlots = new();

    private readonly Plot plotForm;

    private readonly IDictionary<PeakData, PeakDataPlot> plotted = new Dictionary<PeakData, PeakDataPlot>();

    public PeakDataPlotProvider(Plot plotForm)
    {
        this.plotForm = plotForm;
        var sumPeaksLine = PlottableCreator.CreateFunction(x => plotted.Keys.GetPeaksValueAt((float) x), Colors.Red);
        plotForm.Add.Plottable(sumPeaksLine);
    }

    public IReadOnlyCollection<PeakDataPlot> GetPlots(IEnumerable<PeakData> data)
    {
        return data.Select(GetOrCreatePlot).ToArray();
    }

    public void Draw(IEnumerable<PeakDataPlot> data)
    {
        IEnumerable<PeakDataPlot> newPlots;

        lock (plotted)
        {
            newPlots = data
                .Where(p => plotted.TryAdd(p.Peak, p))
                .Select(p => p);
        }

        lock (plotForm)
        {
            foreach (var plot in newPlots)
            {
                foreach (var marker in plot.Markers)
                {
                    plotForm.Add.Plottable(marker);
                }

                plotForm.Add.Plottable(plot.Line);
            }
        }
    }

    public void Erase(IEnumerable<PeakDataPlot> data)
    {
        IEnumerable<PeakDataPlot> removedPlots;

        lock (plotted)
        {
            removedPlots = data
                .Where(p => plotted.Remove(p.Peak))
                .Select(p => p);
        }

        lock (plotForm)
        {
            foreach (var plot in removedPlots)
            {
                foreach (var marker in plot.Markers)
                {
                    plotForm.Remove(marker);
                }

                plotForm.Remove(plot.Line);
            }
        }
    }

    public void Clear()
    {
        lock (plotted)
        {
            lock (plotForm)
            {
                foreach (var plot in plotted.Values)
                {
                    foreach (var marker in plot.Markers)
                    {
                        plotForm.Remove(marker);
                    }

                    plotForm.Remove(plot.Line);
                }
            }

            plotted.Clear();
        }
    }

    private static PeakDataPlot GetOrCreatePlot(PeakData data)
    {
        if (PeakDataPlots.TryGetValue(data, out var plot))
        {
            return plot;
        }

        var newPlot = new PeakDataPlot(data);

        PeakDataPlots.TryAdd(data, newPlot);

        return newPlot;
    }
}
