﻿using Domain;
using Domain.SpectraData;
using Domain.SpectraData.ProcessingInfo;
using Domain.SpectraData.Support;

namespace Controllers;
public class SpectraProcessingController(SpectraGraphics graphics) {
    private readonly SpectraGraphics graphics = graphics;
    protected PeakBordersStorage borders = new();
    public int BordersCount => borders.Count;
    public IEnumerable<PeakBorder> Borders => borders.Borders;

    public void AddBorder(PeakBorder border) {
        lock (borders) {
            if (borders.AddThreadSafe(border)) {
                var plot = border.GetPlot();
                graphics.DrawThreadSafe(plot);
            }
        }
    }

    public void RemoveBorder(PeakBorder border) {
        borders.RemoveThreadSafe(border);
        var plot = border.GetPlot();
        graphics.EraseThreadSafe(plot);
    }

    public PeakInfoSet SetProcessPeaks(DataSet<Spectra> set) {
        var result = new PeakInfoSet();
        Parallel.ForEach(set, spectra => {
            Parallel.ForEach(borders.Borders, border => {
                var info = spectra.ProcessPeak(border);
                result.AddThreadSafe(info);
            });
        });
        return result;
    }

    public PeakInfoSet ProcessPeaks(Spectra spectra) {
        var result = new PeakInfoSet();
        Parallel.ForEach(borders.Borders, border => {
            var info = spectra.ProcessPeak(border);
            result.AddThreadSafe(info);
        });
        return result;
    }

    public static Spectra SubstractBaseline(Spectra spectra) {
        var substracted = spectra.SubstractBaseLine();
        substracted.Name = $"{spectra.Name} b-";
        return substracted;
    }

    public static DataSet<Spectra> SetOnlySubstractBaseline(DataSet<Spectra> set) {
        var substractedSet = new DataSet<Spectra>($"{set.Name} b-");
        Parallel.ForEach(set, spectra => {
            var substracted = SubstractBaseline(spectra);
            substractedSet.AddThreadSafe(substracted);
        });
        return substractedSet;
    }

    public static DataSet<Spectra> SetFullDepthSubstractBaseLin(DataSet<Spectra> set) {
        var copy = set.CopyBranchStructureThreadSafe($"{set.Name} b-");
        Parallel.ForEach(copy.Keys, set => {
            Parallel.ForEach(set, spectra => {
                var substracted = SubstractBaseline(spectra);
                copy[set].AddThreadSafe(substracted);
            });
        });
        return copy[set];
    }

    public void ClearBorders() {
        lock (borders) {
            Parallel.ForEach(borders.Borders, b => graphics.EraseThreadSafe(b.GetPlot()));
            borders.ClearThreadSafe();
        }
    }

    public void RedrawBorders() {
        lock (borders)
            Parallel.ForEach(borders.Borders, b => {
                var plot = b.GetPlot();
                graphics.DrawThreadSafe(plot);
            });
    }
}