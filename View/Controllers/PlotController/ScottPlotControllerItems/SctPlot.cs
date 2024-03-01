using Model.DataFormats;
using ScottPlot;
using ScottPlot.WinForms;
using ScottPlot.Plottables;
using Plot = Model.DataFormats.Plot;
using Color = ScottPlot.Color;

namespace View.Controllers;
public class SctPlot : Plot {
    public static readonly Color HighlightionColor = Colors.Black;
    public static SctPlot PlotSpectra(FormsPlot form, Spectra s) {
        IPlottable plot;
        Color color;
        lock (form.Plot) {
            switch (s.Format) {
                case SpectraFormat.ASP:
                    var delta = ((ASP)s).Info.Delta;
                    var signal = form.Plot.Add.Signal(s.Points.Y.ToArray(), delta);
                    color = signal.Color;
                    plot = signal;
                    break;
                case SpectraFormat.ESP:
                    var signalXY = form.Plot.Add.SignalXY(s.Points.X.ToArray(), s.Points.Y.ToArray());
                    color = signalXY.Color;
                    plot = signalXY;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        return new SctPlot(s, form, plot, color);
    }

    private FormsPlot form;
    public IPlottable Plot { get; private set; }
    public Color Color { get; private set; }
    public bool IsVisible => Plot.IsVisible;

    private SctPlot(Spectra spectra, FormsPlot form, IPlottable plt, Color color)
        : base(spectra) {
        Plot = plt;
        Color = color;
        this.form = form;
    }

    public void ChangeHighlightion(bool isHighlight) {
        lock (form.Plot) {
            form.Plot.Remove(Plot);
            switch (Spectra.Format) {
                case SpectraFormat.ASP:
                    var signal = (Signal)Plot;
                    signal.Color = isHighlight ? HighlightionColor : Color;
                    form.Plot.Add.Plottable(signal);
                    break;
                case SpectraFormat.ESP:
                    var signalXY = (SignalXY)Plot;
                    signalXY.Color = isHighlight ? HighlightionColor : Color;
                    form.Plot.Add.Plottable(signalXY);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public void ChangeVisibility(bool isVisible) => Plot.IsVisible = isVisible;
}
