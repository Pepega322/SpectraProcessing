using System.Collections.Concurrent;
using Controllers.Interfaces;
using Domain.Graphics;
using Domain.InputOutput;
using Domain.SpectraData;
using Domain.Storage;
using MathStatistics.InputOutput;
using MathStatistics.SpectraProcessing;
using Scott.Formats;

namespace Controllers;

public sealed class SpectraProcessingController(
	IPlotDrawer<SctPlot> drawer,
	IPlotBuilder<PeakBorders, PeakBorderPlot> plotBuilder,
	IDataReader<PeakBordersSet> bordersReader
) : ISpectraProcessingController
{
	public IEnumerable<PeakBorders> Borders => borders.Keys;
	private readonly ConcurrentDictionary<PeakBorders, PeakBorderPlot> borders = [];

	public void AddBorder(PeakBorders borders)
	{
		if (this.borders.ContainsKey(borders)) return;
		var plot = plotBuilder.GetPlot(borders);
		drawer.Draw(plot);
		this.borders.TryAdd(borders, plot);
	}

	public void RemoveBorder(PeakBorders borders)
	{
		if (!this.borders.TryGetValue(borders, out var plot)) return;
		drawer.Erase(plot);
		this.borders.TryRemove(borders, out _);
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

	public async Task ImportBorders(string fullname)
	{
		var bordersSet = await Task.Run(() => bordersReader.Get(fullname));
		foreach (var b in bordersSet.Borders)
			AddBorder(b);
	}

	public async Task<SpectrasProcessingResult> ProcessPeaksForSingleSpectra(Spectra spectra)
	{
		var result = new SpectrasProcessingResult();
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

	public async Task<SpectrasProcessingResult> ProcessPeaksForSpectraSet(DataSet<Spectra> set)
	{
		var result = new SpectrasProcessingResult();
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

	public async Task<Spectra> GetAverageSpectra(IEnumerable<Spectra> set)
	{
		return await Task.Run(() => set.GetAverageSpectra());
	}
}