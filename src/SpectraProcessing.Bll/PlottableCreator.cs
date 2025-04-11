using ScottPlot;
using ScottPlot.Plottables;
using SpectraProcessing.Bll.Models.ScottPlot.Plottables;

namespace SpectraProcessing.Bll;

public static class PlottableCreator
{
    private const float lineWidth = 2f;
    private const float markerSize = 20f;

    public static Signal CreateSignal(float[] ys, float delta, Color? color = null)
    {
        using var builder = new Plot();

        var signal = builder.Add.Signal(ys, delta, color);

        signal.LineWidth = lineWidth;

        return signal;
    }

    public static SignalXY CreateSignalXY(float[] xs, float[] ys, Color? color = null)
    {
        using var builder = new Plot();

        var signalXy = builder.Add.SignalXY(xs, ys, color);

        signalXy.LineWidth = lineWidth;

        return signalXy;
    }

    public static FunctionPlot CreateFunction(Func<double, double> func, Color? color = null)
    {
        using var builder = new Plot();

        var function = builder.Add.Function(func);

        if (color is not null)
        {
            function.LineColor = color.Value;
        }

        function.LineWidth = lineWidth;

        return function;
    }

    public static DraggableMarker CreateDraggableMarker(
        double x,
        double y,
        MarkerShape shape,
        Color? color = null)
    {
        using var builder = new Plot();

        var marker = builder.Add.Marker(x, y, shape, markerSize, color);

        return new DraggableMarker(marker);
    }
}
