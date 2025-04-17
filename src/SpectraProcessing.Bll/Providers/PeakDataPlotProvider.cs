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

    public IEnumerable<PeakDataPlot> GetPlots(IEnumerable<PeakData> data)
    {
        return data.Select(GetOrCreatePlot);
    }

    public IEnumerable<PeakDataPlot> Draw(IEnumerable<PeakData> data)
    {
        IEnumerable<PeakDataPlot> newPlots;

        lock (plotted)
        {
            newPlots = data
                .Select(d => (Peak: d, Plot: GetOrCreatePlot(d)))
                .Where(p => plotted.TryAdd(p.Peak, p.Plot))
                .Select(p => p.Plot);
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

                yield return plot;
                ;
            }
        }
    }

    public IEnumerable<PeakDataPlot> Erase(IEnumerable<PeakData> data)
    {
        IEnumerable<PeakDataPlot> removedPlots;

        lock (plotted)
        {
            removedPlots = data
                .Select(d => (Peak: d, Plot: GetOrCreatePlot(d)))
                .Where(p => plotted.Remove(p.Peak))
                .Select(p => p.Plot);
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

                yield return plot;
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
