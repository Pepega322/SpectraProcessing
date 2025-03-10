using ScottPlot;
using Plot = SpectraProcessing.Domain.Graphics.Plot;

namespace SpectraProcessing.Graphics.Formats;

public abstract class SctPlot : Plot
{
    public abstract string Name { get; protected set; }

    public abstract Color PreviousColor { get; protected set; }

    public abstract IReadOnlyCollection<IPlottable> Plottables { get; }

    public abstract void ChangeColor(Color color);
}
