using ScottPlot.WinForms;
using ScottPlot.Plottables;
using Model.DataFormats;

namespace View.Controllers;

public record SctPlotPeakBorder(float Left, float Right, VerticalLine LeftLine, VerticalLine RigthLine) : PeakBorder(Left, Right) {
    public static SctPlotPeakBorder AddOnPlot(FormsPlot form, float left, float right) {
        lock (form.Plot) {
            var leftLine = form.Plot.Add.VerticalLine(left, 1);
            var rigthtLine = form.Plot.Add.VerticalLine(right, 1, leftLine.Color);
            form.Plot.Remove(leftLine);
            form.Plot.Remove(rigthtLine);
            return new SctPlotPeakBorder(left, right, leftLine, rigthtLine);
        }
    }
}