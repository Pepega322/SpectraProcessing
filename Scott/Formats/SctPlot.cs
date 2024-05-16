using ScottPlot;
using Graphics_Plot = Domain.Graphics.Plot;

namespace Scott.Formats;

public abstract class SctPlot : Graphics_Plot
{
	public abstract string Name { get; protected set; }
	public Color PreviousColor;
	public abstract IEnumerable<IPlottable> GetPlottables();

	public abstract void ChangeColor(Color color);
}