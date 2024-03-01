using Model.DataFormats;
using Model.DataStorages;
using Model.Controllers;
using ScottPlot.Palettes;
using ScottPlot.WinForms;
using Plot = Model.DataFormats.Plot;
using Model.MathHelper;

namespace View.Controllers;
internal class ScottPlotController : PlotController, ITree {
    private FormsPlot form { get; set; }
    private PlotSet? highlightedSet { get; set; }
    private Plot? highlightedPlot { get; set; }

    public ScottPlotController(FormsPlot form)
        : base(new SctPlotStorage("Single Plots"), new ScotPlotPeakController(form)) {
        this.form = form;
        form.Plot.Add.Palette = new Category20();
        form.Plot.XLabel("Raman shift, cm-1");
        form.Plot.YLabel("Intensity");
    }

    public override async Task AddDataPlotAsync(Data data)
        => await Task.Run(() => PlotDataTo(data, storage.DefaultSet));

    public override async Task AddDataSetPlotAsync(DataSet set, bool toDefault) {
        var destination = toDefault ? storage.DefaultSet : new SctPlotSet(set.Name);
        await Task.Run(() => Parallel.ForEach(set, d => PlotDataTo(d, destination)));
        if (!toDefault)
            storage.Add(set.Name, destination);
    }

    public override async Task RemovePlotAsync(Plot plot, PlotSet owner)
        => await Task.Run(() => RemovePlotFrom(plot, owner));

    public override async Task RemovePlotSetAsync(PlotSet set) {
        storage.Remove(set.Name);
        await Task.Run(() => Parallel.ForEach((IEnumerable<Plot>)set, p => RemovePlotFrom(p, set)));
    }

    public override async Task ChangePlotVisibilityAsync(Plot plot, bool isVisible)
        => await Task.Run(() => ChangeVisibility(plot, isVisible));

    public override async Task ChangePlotSetVisibilityAsync(PlotSet set, bool isVisible)
        => await Task.Run(() => Parallel.ForEach((IEnumerable<Plot>)set, plot => ChangeVisibility(plot, isVisible)));

    public override async Task ChangePlotHighlightionAsync(Plot plot) {
        if (highlightedPlot != null)
            await Task.Run(() => ChangeHighlightion(highlightedPlot, false));

        if (highlightedPlot != plot) {
            highlightedPlot = plot;
            await Task.Run(() => ChangeHighlightion(highlightedPlot, true));
        }
        else highlightedPlot = null;
    }

    public override async Task ChangePlotSetHighlightionAsync(PlotSet set) {
        if (highlightedSet != null)
            await Task.Run(() => Parallel.ForEach((IEnumerable<Plot>)highlightedSet, plot => ChangeHighlightion(plot, false)));

        if (highlightedSet != set) {
            highlightedSet = set;
            await Task.Run(() => Parallel.ForEach((IEnumerable<Plot>)highlightedSet, plot => ChangeHighlightion(plot, true)));
        }
        else highlightedSet = null;
    }

    public override async Task<CalculatedPeaks> ProcessPlotSet(PlotSet set) {
        var info = new CalculatedPeaks($"{set.Name} (set peaks)");
        foreach (var plot in (IEnumerable<Plot>)set)
            await Task.Run(() => Parallel.ForEach(bordersControler.Borders, b => info.Add(plot.Spectra.ProcessPeak(b))));
        return info;
    }

    public override async Task<CalculatedPeaks> ProcessPlot(Plot plot) {
        var info = new CalculatedPeaks($"{plot.Name} (peaks)");
        await Task.Run(() => Parallel.ForEach(bordersControler.Borders, b => info.Add(plot.Spectra.ProcessPeak(b))));
        return info;
    }

    public override void Refresh() {
        form.Refresh();
    }

    public override void Resize() {
        form.Plot.Axes.AutoScaleX();
        form.Plot.Axes.AutoScaleY();
    }

    public override void Clear() {
        storage.Clear();
        highlightedPlot = null;
        highlightedSet = null;
        form.Plot.Clear();
        bordersControler.RedrawBorders();
    }

    private void ChangeHighlightion(Plot plot, bool isHighlighted) {
        if (plot is not SctPlot sct) return;
        sct.ChangeHighlightion(isHighlighted);
    }

    private void RemovePlotFrom(Plot plot, PlotSet owner) {
        if (plot is not SctPlot plt) return;
        lock (form.Plot) form.Plot.Remove(plt.Plot);
        owner.Remove(plot);
    }

    private void PlotDataTo(Data data, DataSet destination) {
        if (data is not Spectra spectra || destination.Contains(spectra)) return;
        var plot = SctPlot.PlotSpectra(form, spectra);
        destination.Add(plot);
    }

    private void ChangeVisibility(Data data, bool isVisible) {
        if (data is not SctPlot plot) return;
        plot.ChangeVisibility(isVisible);
    }

    public IEnumerable<TreeNode> GetTree() {
        foreach (var pair in storage) {
            var setNode = new TreeNode {
                Text = pair.Key,
                Tag = (PlotSet)pair.Value
            };
            var plots = pair.Value.Select(s => (SctPlot)s);
            setNode.Checked = plots.All(p => p.IsVisible);
            foreach (var plot in plots.OrderByDescending(x => x.Name)) {
                var node = new TreeNode {
                    Text = plot.Name,
                    Tag = plot,
                    Checked = plot.IsVisible
                };
                setNode.Nodes.Add(node);
            }
            if (setNode.Nodes.Count > 0)
                yield return setNode;
        }
    }

    #region borderControllerMethods

    public override async Task SetCoordinates(float xScreen, float yScreen) {
        await Task.Run(() => {
            var c = form.Plot.GetCoordinates(xScreen, yScreen);
            bordersControler.SetCoordinates((float)c.X, (float)c.Y);
        });
    }

    public override async Task<bool> AddBorder()
        => await bordersControler.AddBorder();

    public override async Task<bool> DeleteLastBorder()
        => await bordersControler.DeleteLast();

    public override async Task<bool> ClearBorders()
        => await Task.Run(bordersControler.Clear);

    #endregion
}