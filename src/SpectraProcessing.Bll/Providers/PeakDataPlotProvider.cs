﻿using ScottPlot;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Providers;

internal sealed class PeakDataPlotProvider(Plot plotForm) : IDataPlotProvider<PeakData, PeakDataPlot>
{
    private readonly IDictionary<PeakData, PeakDataPlot> plotted = new Dictionary<PeakData, PeakDataPlot>();

    public Task<bool> IsDrew(PeakData data)
    {
        lock (plotted)
        {
            return Task.FromResult(plotted.ContainsKey(data));
        }
    }

    public Task<IReadOnlyCollection<PeakDataPlot>> Draw(IReadOnlyCollection<PeakData> data)
    {
        IReadOnlyCollection<PeakDataPlot> plots = data.Select(GetOrCreatePlot).ToArray();

        IReadOnlyCollection<PeakDataPlot> newPlots;

        lock (plotted)
        {
            newPlots = plots
                .Where(p => plotted.TryAdd(p.Peak, p))
                .ToArray();

            if (newPlots.IsEmpty())
            {
                return Task.FromResult(plots);
            }
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

        return Task.FromResult(plots);
    }

    public Task<IReadOnlyCollection<PeakDataPlot>> Erase(IReadOnlyCollection<PeakData> data)
    {
        IReadOnlyCollection<PeakDataPlot> plots = data.Select(GetOrCreatePlot).ToArray();

        IReadOnlyCollection<PeakDataPlot> removedPlots;

        lock (plotted)
        {
            removedPlots = plots
                .Where(p => plotted.Remove(p.Peak))
                .ToArray();

            if (removedPlots.IsEmpty())
            {
                return Task.FromResult(plots);
            }
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

        return Task.FromResult(plots);
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

        return Task.CompletedTask;
    }

    private PeakDataPlot GetOrCreatePlot(PeakData data)
    {
        lock (plotted)
        {
            if (plotted.TryGetValue(data, out var plot))
            {
                return plot;
            }
        }

        return new PeakDataPlot(data);
    }
}
