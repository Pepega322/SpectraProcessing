namespace Domain.SpectraData;

public abstract class SpectraGraphics
{
	protected readonly HashSet<SpectraPlot> Plots = [];
	protected int PlotCount => Plots.Count;

	public abstract void DrawThreadSafe(SpectraPlot plot);
	public abstract void EraseThreadSafe(SpectraPlot plot);
	public abstract void ChangeHighlightion(SpectraPlot visual, bool isHighlighted);
	public abstract void ResizeThreadSafe();
	public abstract void ClearThreadSafe();
}