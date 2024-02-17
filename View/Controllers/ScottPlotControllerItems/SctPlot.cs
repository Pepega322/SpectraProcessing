using Model.DataFormats;
using ScottPlot;
using ScottPlot.WinForms;
using ScottPlot.Plottables;
using Plot = Model.DataFormats.Plot;
using Color = ScottPlot.Color;

namespace View.Controllers;
public class SctPlot : Plot {
    private static Dictionary<IReadOnlyList<float>, float[]> xSPlots = [];
    private static Color HighlightionColor = Colors.Black;

    private FormsPlot form;
    public SctPlotFromats Format { get; private set; }
    public IPlottable Plot { get; private set; }
    public Color Color { get; private set; }
    public bool IsVisible => Plot.IsVisible;

    private SctPlot(SctPlotFromats format, Spectra spectra, FormsPlot form, IPlottable plt, Color color)
        : base(spectra) {
        Format = format;
        Plot = plt;
        Color = color;
        this.form = form;
    }

    public static SctPlot GetPlot(SctPlotFromats format, Spectra spectra, FormsPlot form) {
        var (xS, yS) = spectra.GetPoints();
        lock (xSPlots) {
            if (!xSPlots.ContainsKey(xS))
                xSPlots.Add(xS, xS.ToArray());
        }
        IPlottable plot;
        Color color;
        lock (form.Plot) {
            switch (format) {
                case SctPlotFromats.Signal:
                    var signal = form.Plot.Add.Signal(yS, ((ASP)spectra).Delta);
                    color = signal.Color;
                    plot = signal;
                    break;
                case SctPlotFromats.SignalXY:
                    var signalXY = form.Plot.Add.SignalXY(xSPlots[xS], yS);
                    color = signalXY.Color;
                    plot = signalXY;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        return new SctPlot(format, spectra, form, plot, color);
    }

    public void ChangeHighlightion(bool isHighlight) {
        form.Plot.Remove(Plot);
        switch (Format) {
            case SctPlotFromats.Signal:
                var signal = (Signal)Plot;
                signal.Color = isHighlight ? HighlightionColor : Color;
                form.Plot.Add.Plottable(signal);
                break;
            case SctPlotFromats.SignalXY:
                var signalXY = (SignalXY)Plot;
                signalXY.Color = isHighlight ? HighlightionColor : Color;
                form.Plot.Add.Plottable(signalXY);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void ChangeVisibility(bool isVisible) => Plot.IsVisible = isVisible;
}
