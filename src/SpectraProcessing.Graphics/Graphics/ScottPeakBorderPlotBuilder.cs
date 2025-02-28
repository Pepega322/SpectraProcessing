using ScottPlot;
using SpectraProcessing.Domain.Graphics;
using SpectraProcessing.Graphics.Formats;
using SpectraProcessing.MathStatistics.SpectraProcessing;
using Plot = ScottPlot.Plot;

namespace SpectraProcessing.Graphics.Graphics;

public class ScottPeakBorderPlotBuilder(IPalette palette) : IPlotBuilder<PeakBorders, PeakBorderPlot>
{
    private readonly Plot builder = new();
    private int counter;

    public PeakBorderPlot GetPlot(PeakBorders plottableData)
    {
        var color = palette.GetColor(counter);
        Interlocked.Increment(ref counter);
        var left = builder.Add.VerticalLine(plottableData.XStart, 1, color);
        var right = builder.Add.VerticalLine(plottableData.XEnd, 1, color);
        return new PeakBorderPlot(plottableData, left, right);
    }
}
