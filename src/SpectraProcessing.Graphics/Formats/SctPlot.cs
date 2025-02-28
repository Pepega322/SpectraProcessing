using ScottPlot;
using Plot = SpectraProcessing.Domain.Graphics.Plot;

namespace SpectraProcessing.Graphics.Formats;

public abstract class SctPlot : Plot
{
    public abstract string Name { get; protected set; }
    public Color PreviousColor;
    public abstract IEnumerable<IPlottable> GetPlottables();

    public abstract void ChangeColor(Color color);
}
