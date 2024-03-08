using Domain.SpectraData;
using ScottPlot;

namespace Scott.GraphicsData;
public abstract class Plottable : SpectraPlot {
    protected readonly static Plot builder = new();

    public Color DefaultColor { get; protected set; }
    public Color Color { get; protected set; }

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