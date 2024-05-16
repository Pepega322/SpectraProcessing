using System.Collections.Concurrent;
using Controllers.Interfaces;
using Domain.Graphics;
using Domain.SpectraData;
using Domain.SpectraData.Processing;
using Domain.Storage;
using Scott.Formats;

namespace Controllers;

public sealed class SpectraProcessingController(
	IPlotDrawer<SctPlot> drawer,
	IPlotBuilder<PeakBorder, PeakBorderPlot> plotBuilder
) : ISpectraProcessingController
{
	public IEnumerable<PeakBorder> Borders => borders.Keys;
	private readonly ConcurrentDictionary<PeakBorder, PeakBorderPlot> borders = [];

	public void AddBorder(PeakBorder border)
	{
		if (borders.ContainsKey(border)) return;
		var plot = plotBuilder.GetPlot(border);
		drawer.Draw(plot);
		borders.TryAdd(border, plot);
	}

	public void RemoveBorder(PeakBorder border)
	{
		if (!borders.TryGetValue(border, out var plot)) return;
		drawer.Erase(plot);
		borders.TryRemove(border, out _);
	}

	public void ClearBorders()
	{
		foreach (var plot in borders.Values)
		{
			drawer.Erase(plot);
		}

		borders.Clear();
	}

	public void RedrawBorders()
	{
		foreach (var plot in borders.Values)
		{
			drawer.Erase(plot);
		}
	}

	public PeakInfoSet ProcessPeaksForSingleSpectra(Spectra spectra)
	{
		var result = new PeakInfoSet();
		Parallel.ForEach(borders.Keys, border =>
		{
			var info = spectra.ProcessPeak(border);
			result.Add(info);
		});
		return result;
	}

	public PeakInfoSet ProcessPeaksForSpectraSet(DataSet<Spectra> set)
	{
		var result = new PeakInfoSet();
		Parallel.ForEach(set, spectra =>
		{
			foreach (var border in Borders)
			{
				var info = spectra.ProcessPeak(border);
				result.Add(info);
			}
		});
		return result;
	}

	public Spectra SubstractBaselineForSingleSpectra(Spectra spectra)
	{
		var substracted = spectra.SubstractBaseLine();
		substracted.Name = $"{spectra.Name} b-";
		return substracted;
	}

	public DataSet<Spectra> SubstractBaselineForSingleSpectraSet(DataSet<Spectra> set)
	{
		var substractedSet = new DataSet<Spectra>($"{set.Name} b-");
		Parallel.ForEach(set, spectra =>
		{
			var substracted = SubstractBaselineForSingleSpectra(spectra);
			substractedSet.AddThreadSafe(substracted);
		});
		return substractedSet;
	}

	public DataSet<Spectra> SubstractBaselineForSpectraSetFullDepth(DataSet<Spectra> set)
	{
		var refToCopy = set.CopyBranchStructureThreadSafe($"{set.Name} b-");
		foreach (var subset in refToCopy.Keys)
		{
			Parallel.ForEach(subset, spectra =>
			{
				var substracted = SubstractBaselineForSingleSpectra(spectra);
				refToCopy[set].AddThreadSafe(substracted);
			});
		}

		return refToCopy[set];
	}

	public Spectra GetAverageSpectraForSet(DataSet<Spectra> set)
	{
		var average = set.GetAverageSpectra(out _);
		average.Name = $"{set.Name} (average)";
		return average;
	}
}