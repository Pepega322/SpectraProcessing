using ScottPlot;
using Domain.SpectraData;

namespace Scott.GraphicsData;
public class ScottSpectraGraphics(Plot plot, IPalette palette) : SpectraGraphics {
    private readonly static Color HighlightionColor = Colors.Black;
    private readonly Plot plot = plot;
    private readonly IPalette palette = palette;
    private Color NewColor => palette.GetColor(PlotCount);

    public override void DrawThreadSafe(SpectraPlot visual) {
        if (visual is not Plottable plt) return;
        lock (plots) {
            if (!plots.Add(plt)) return;
            plt.SetColor(NewColor);
            plt.ChangeVisibility(true);
        }
        lock (plot) {
            foreach (IPlottable p in plt.GetPlots())
                plot.Add.Plottable(p);
        }
    }

    public override void EraseThreadSafe(SpectraPlot visual) {
        if (visual is not Plottable plt) return;
        lock (plots) {
            if (!plots.Remove(plt)) return;
            plt.SetColor(NewColor);
        }
        lock (plot) {
            foreach (IPlottable p in plt.GetPlots())
                plot.Remove(p);
        }
    }

    public override void ChangeHighlightion(SpectraPlot plot, bool isHighlighted) {
        if (plot is not Plottable plt) return;
        if (isHighlighted) {
            PushOnTopThreadSafe(plot);
            plt.RememberColor();
            plt.SetColor(HighlightionColor);
        }
        else {
            plt.SetColor(plt.DefaultColor);
        }
    }

    public override void ResizeThreadSafe() {
        lock (plot) {
            plot.Axes.AutoScaleX();
            plot.Axes.AutoScaleY();
        }
    }

    public override void ClearThreadSafe() {
        lock (plots) plots.Clear();
        lock (plot) plot.Clear();
    }

    private void PushOnTopThreadSafe(SpectraPlot visual) {
        if (visual is not Plottable plt || !plots.Contains(plt)) return;
        lock (plot) {
            foreach (IPlottable p in plt.GetPlots()) {
                plot.Remove(p);
                plot.Add.Plottable(p);
            }
        }
    }
}
