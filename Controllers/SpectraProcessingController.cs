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

	public async Task<PeakInfoSet> ProcessPeaksForSingleSpectra(Spectra spectra)
	{
		var result = new PeakInfoSet();
		await Task.Run(() =>
		{
			Parallel.ForEach(borders.Keys, border =>
			{
				var info = spectra.ProcessPeak(border);
				result.Add(info);
			});
		});
		return result;
	}

	public async Task<PeakInfoSet> ProcessPeaksForSpectraSet(DataSet<Spectra> set)
	{
		var result = new PeakInfoSet();
		await Task.Run(() =>
		{
			Parallel.ForEach(set, spectra =>
			{
				foreach (var border in Borders)
				{
					var info = spectra.ProcessPeak(border);
					result.Add(info);
				}
			});
		});
		return result;
	}

	public async Task<Spectra> SubstractBaseline(Spectra spectra)
	{
		var substracted = await Task.Run(spectra.SubstractBaseLine);
		substracted.Name = $"{spectra.Name} b-";
		return substracted;
	}

	public async Task<Spectra[]> SubstractBaseline(IEnumerable<Spectra> set)
	{
		var substractedSet = new ConcurrentBag<Spectra>();
		await Task.Run(() =>
		{
			Parallel.ForEach(set, spectra =>
			{
				var substracted = spectra.SubstractBaseLine();
				substractedSet.Add(substracted);
			});
		});
		return substractedSet.ToArray();
	}

	// public DataSet<Spectra> SubstractBaselineForSpectraSetFullDepth(DataSet<Spectra> set)
	// {
	// 	var refToCopy = set.CopyBranchStructureThreadSafe($"{set.Name} b-");
	// 	foreach (var subset in refToCopy.Keys)
	// 	{
	// 		Parallel.ForEach(subset, spectra =>
	// 		{
	// 			var substracted = SubstractBaseline(spectra);
	// 			refToCopy[set].AddThreadSafe(substracted);
	// 		});
	// 	}
	//
	// 	return refToCopy[set];
	// }

	public async Task<Spectra> GetAverageSpectra(IEnumerable<Spectra> set)
	{
		return await Task.Run(() => set.GetAverageSpectra(out _));
	}
}