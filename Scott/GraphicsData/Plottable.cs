using Domain.SpectraData;
using ScottPlot;

namespace Scott.GraphicsData;

public abstract class Plottable : SpectraPlot {
	protected static readonly Plot Builder = new();

	public Color DefaultColor { get; private set; }
	protected Color Color { get; set; }

	public abstract void SetColor(Color color);

	public abstract IEnumerable<IPlottable> GetPlots();

	public override void ChangeVisibility(bool isVisible) {
		IsVisible = isVisible;
		foreach (IPlottable plot in GetPlots())
			plot.IsVisible = IsVisible;
	}

	public void RememberColor() {
		DefaultColor = Color;
	}
}