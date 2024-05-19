using Controllers.Interfaces;
using Domain.Graphics;
using Domain.Storage;
using Scott.Formats;

namespace Controllers;

public sealed class ScottSpectraGraphicsController(IPlotDrawer<SctPlot> drawer) : IGraphicsController<SpectraPlot>
{
	private SpectraPlot? highlightedData;
	private DataSet<SpectraPlot>? highlightedSet;

	public void DrawData(SpectraPlot plot)
	{
		drawer.Draw(plot);
	}

	public void DrawDataSet(DataSet<SpectraPlot> set)
	{
		foreach (var plot in set.Data)
		{
			DrawData(plot);
		}
	}

	public void EraseData(SpectraPlot plot)
	{
		drawer.Erase(plot);
	}

	public void EraseDataSet(DataSet<SpectraPlot> set)
	{
		foreach (var plot in set.Data)
		{
			EraseData(plot);
		}
	}

	public void ChangeDataVisibility(SpectraPlot plot, bool isVisible)
	{
		drawer.SetVisibility(plot, isVisible);
	}

	public void ChangeDataSetVisibility(DataSet<SpectraPlot> set, bool isVisible)
	{
		Parallel.ForEach(set.Data, plot => ChangeDataVisibility(plot, isVisible));
	}

	public void HighlightData(SpectraPlot plot)
	{
		if (highlightedData != null)
			SetHighlighting(highlightedData, false);

		if (Equals(highlightedData, plot)) highlightedData = null;
		else
		{
			highlightedData = plot;
			SetHighlighting(plot, true);
		}
	}

	public void HighlightDataSet(DataSet<SpectraPlot> set)
	{
		if (highlightedSet != null)
			Parallel.ForEach(highlightedSet.Data, data => SetHighlighting(data, false));

		if (Equals(highlightedSet, set)) highlightedSet = null;
		else
		{
			highlightedSet = set;
			Parallel.ForEach(set.Data, data => SetHighlighting(data, true));
		}
	}

	public void ClearArea()
	{
		drawer.Clear();
		highlightedData = null;
		highlightedSet = null;
	}

	public void ResizeArea()
	{
		drawer.Resize();
	}

	private void SetHighlighting(SpectraPlot plot, bool isHighlighted)
	{
		drawer.SetHighlight(plot, isHighlighted);
	}
}