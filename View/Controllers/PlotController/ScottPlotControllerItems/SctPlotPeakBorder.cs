using ScottPlot.WinForms;
using ScottPlot.Plottables;
using Model.DataFormats;

namespace View.Controllers;

public class SctPlotPeakBorder : PeakBorder {
    public FormsPlot Form { get; set; }
    public readonly VerticalLine LeftLine;
    public readonly VerticalLine RigthLine;

    public SctPlotPeakBorder(FormsPlot form, float left, float right)
        : base(left, right) {
        Form = form;
        LeftLine = form.Plot.Add.VerticalLine(Left, 1);
        RigthLine = form.Plot.Add.VerticalLine(Rigth, 1, LeftLine.Color);
    }

    public void RemoveFromPlot() {
        lock (Form.Plot) {
            Form.Plot.Remove(LeftLine);
            Form.Plot.Remove(RigthLine);
        }
    }
}