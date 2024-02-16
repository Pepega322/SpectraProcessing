using Model.DataFormats;
using ScottPlot;
using ScottPlot.WinForms;
using ScottPlot.Palettes;
using ScottPlot.Plottables;
using PlotColor = ScottPlot.Color;

namespace View.Controllers;
public class ScottPlotController {
    private readonly FormsPlot form;
    private readonly Dictionary<IReadOnlyList<float>, float[]> xSPlots;
    private readonly Dictionary<Data, IPlottable> plots;

    private PlotColor colorOnSelect;
    private IPlottable? plotOnSelect = null!;

    public ScottPlotController(FormsPlot f) {
        form = f;
        form.Plot.Add.Palette = new Category20();
        form.Plot.XLabel("Raman shift, cm-1");
        form.Plot.YLabel("Intensity");
        xSPlots = [];
        plots = [];
    }

    public void Clear() {
        plots.Clear();
        form.Plot.Clear();
        form.Refresh();
    }

    public async Task PlotSet(ISet<Data> dataToBePlotted) {
        await Task.Run(() => UpdatePlot(dataToBePlotted));
        form.Refresh();
        form.Plot.Axes.AutoScale();
    }

    public void ChangeVisibility(Data data, bool isVisible) {
        if (plots.ContainsKey(data)) {
            plots[data].IsVisible = isVisible;
            form.Refresh();
        }
    }

    public async void SelectPlot(Data data) {
        await Task.Run(() => HighlighPlot(data));
        form.Refresh();
    }

    private void HighlighPlot(Data data) {
        if (!plots.ContainsKey(data) || plots[data] == plotOnSelect) return;
        if (plotOnSelect is not null)
            ChangeColor(plotOnSelect, colorOnSelect, out PlotColor t);
        plotOnSelect = plots[data];
        ChangeColor(plotOnSelect, Colors.Black, out PlotColor original);
        colorOnSelect = original;
        form.Plot.Remove(plotOnSelect);
        form.Plot.PlottableList.Add(plotOnSelect);
    }

    private void ChangeColor(IPlottable plot, PlotColor color, out PlotColor original) {
        if (plot is Signal signal) {
            original = signal.Color;
            signal.Color = color;
        }
        else if (plot is SignalXY xySignal) {
            original = xySignal.Color;
            xySignal.Color = color;
        }
        else throw new NotImplementedException();
    }

    private void UpdatePlot(ISet<Data> dataToBePlotted) {
        var toAdd = dataToBePlotted
            .Where(d => !plots.ContainsKey(d))
            .ToArray();
        var toRemove = plots.Keys
            .Where(d => !dataToBePlotted.Contains(d))
            .ToArray();
        Parallel.ForEach(toAdd, Add);
        Parallel.ForEach(toRemove, Remove);
    }

    private void Add(Data data) {
        if (plots.ContainsKey(data)) return;
        if (data is Spectra spectra)
            PlotSpectra(spectra);
    }

    private void PlotSpectra(Spectra spectra) {
        var (xS, yS) = spectra.GetPoints();
        lock (xSPlots) {
            if (!xSPlots.ContainsKey(xS))
                xSPlots.Add(xS, xS.ToArray());
        }

        IPlottable plot;
        lock (form.Plot) {
            if (spectra is ASP asp)
                plot = form.Plot.Add.Signal(yS, asp.Delta);
            else if (spectra is ESP)
                plot = form.Plot.Add.SignalXY(xSPlots[xS], yS);
            else
                throw new NotImplementedException();
        }

        lock (plots)
            plots.Add(spectra, plot);
    }

    private void Remove(Data data) {
        lock (plots) {
            if (plots.ContainsKey(data)) {
                lock (form) form.Plot.Remove(plots[data]);
                plots.Remove(data);
            }
        }
    }

    public IEnumerable<TreeNode> GetPlotNodes() {
        foreach (var pair in plots)
            yield return new TreeNode {
                Text = pair.Key.Name,
                Tag = pair.Key,
                Checked = pair.Value.IsVisible,
            };
    }
}
