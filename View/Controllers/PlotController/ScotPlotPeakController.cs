using Model.Controllers;
using Model.DataFormats;
using ScottPlot.WinForms;

namespace View.Controllers;
public class ScotPlotPeakController : PeakBordersController {
    private FormsPlot form { get; init; }

    public ScotPlotPeakController(FormsPlot form) {
        this.form = form;
    }

    public override async Task<bool> AddBorder() {
        var start = await GetXCoordinateByClick();
        var end = await GetXCoordinateByClick();
        var border = await Task.Run(() => SctPlotPeakBorder.AddOnPlot(form, start, end));
        return await Task.Run(() => AddBorder(border));
    }

    protected override void DrawOnPlot(PeakBorder border) {
        if (border is not SctPlotPeakBorder b) throw new Exception();
        form.Plot.Add.Plottable(b.LeftLine);
        form.Plot.Add.Plottable(b.RigthLine);
    }

    protected override async Task<float> GetXCoordinateByClick() {
        var task = new Task<float>(() => Coordinates.X);
        MouseEventHandler handler = (sender, e) => task.Start();
        form.MouseDown += handler;
        var res = await task;
        form.MouseDown -= handler;
        return res;
    }

    protected override async Task<float> GetXCoordinateByKeyDown() {
        var task = new Task<float>(() => Coordinates.X);
        KeyEventHandler handler = (sender, e) => {
            if (e.KeyData == Keys.S) task.Start();
        };
        form.KeyDown += handler;
        var res = await task;
        form.KeyDown -= handler;
        return res;
    }

    protected override void WipeFromPlot(PeakBorder border) {
        if (border is not SctPlotPeakBorder b) throw new Exception();
        form.Plot.Remove(b.LeftLine);
        form.Plot.Remove(b.RigthLine);
    }
}
