using System.Collections.Concurrent;
using ScottPlot;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.Spectra;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Providers;

internal sealed class SpectraDataPlotProvider(
    Plot plotForm,
    IPalette palette
) : IDataPlotProvider<SpectraData, SpectraDataPlot>
{
    private static readonly ConcurrentDictionary<SpectraData, SpectraDataPlot> SpectraDataPlots = new();

    private static int _counter;

    private readonly IDictionary<SpectraData, SpectraDataPlot> plotted = new Dictionary<SpectraData, SpectraDataPlot>();

    public Task<bool> IsDrew(SpectraData data)
    {
        lock (plotted)
        {
            return Task.FromResult(plotted.ContainsKey(data));
        }
    }

    public Task<IReadOnlyCollection<SpectraDataPlot>> Draw(IReadOnlyCollection<SpectraData> data)
    {
        IReadOnlyCollection<SpectraDataPlot> plots = data.Select(GetOrCreatePlot).ToArray();

        IReadOnlyCollection<SpectraDataPlot> newPlots;

        lock (plotted)
        {
            newPlots = plots
                .Where(p => plotted.TryAdd(p.SpectraData, p))
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
                plotForm.Add.Plottable(plot.Plottable);
            }
        }

        return Task.FromResult(plots);
    }

    public Task<IReadOnlyCollection<SpectraDataPlot>> Erase(IReadOnlyCollection<SpectraData> data)
    {
        IReadOnlyCollection<SpectraDataPlot> plots = data.Select(GetOrCreatePlot).ToArray();

        IReadOnlyCollection<SpectraDataPlot> removedPlots;

        lock (plotted)
        {
            removedPlots = plots
                .Where(p => plotted.Remove(p.SpectraData))
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
                plotForm.Remove(plot.Plottable);
            }
        }

        return Task.FromResult(plots);
    }

    public Task PushOnTop(IReadOnlyCollection<SpectraData> data)
    {
        lock (plotForm)
        {
            foreach (var d in data)
            {
                if (plotted.TryGetValue(d, out var plot) is false)
                {
                    continue;
                }

                plotForm.Remove(plot.Plottable);
                plotForm.Add.Plottable(plot.Plottable);
            }
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
        lock (plotted)
        {
            lock (plotForm)
            {
                foreach (var plot in plotted.Values)
                {
                    plotForm.Remove(plot.Plottable);
                }
            }

            plotted.Clear();
        }

        return Task.CompletedTask;
    }

    private SpectraDataPlot GetOrCreatePlot(SpectraData data)
    {
        if (SpectraDataPlots.TryGetValue(data, out var plot))
        {
            return plot;
        }

        var color = palette.GetColor(Interlocked.Increment(ref _counter));

        SpectraDataPlot newPlot = data switch
        {
            AspSpectraData asp => new AspSpectraDataPlot(asp),
            EspSpectraData esp => new EspSpectraDataPlot(esp),
            _                  => throw new NotSupportedException(data.GetType().Name + " is not supported"),
        };

        newPlot.ChangeColor(color);

        SpectraDataPlots.TryAdd(data, newPlot);

        return newPlot;
    }
}
