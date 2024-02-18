using Model.DataFormats;
using Model.DataStorages;
using Model.Controllers;
using ScottPlot.Palettes;
using ScottPlot.WinForms;

namespace View.Controllers;
internal class ScottPlotController : PlotController, ITree {
    private FormsPlot form;
    private PlotSet? HighlightedSet;
    private Plot? HighlightedPlot;

    public ScottPlotController(FormsPlot form) : base(new SctPlotStorage()) {
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

    private void PlotDataTo(Data data, DataSet destination) {
        if (data is not Spectra spectra || destination.Contains(spectra)) return;
        var format = SctPlot.GetPlotFormat(spectra);
        var plot = SctPlot.PlotSpectra(format, spectra, form);
        destination.Add(plot);
    }

    public override async Task RemovePlotAsync(Plot plot, PlotSet owner)
        => await Task.Run(() => RemovePlotFrom(plot, owner));

    public override async Task RemovePlotSetAsync(PlotSet set) {
        storage.Remove(set.Name);
        await Task.Run(() => Parallel.ForEach((IEnumerable<Plot>)set, p => RemovePlotFrom(p, set)));
    }

    private void RemovePlotFrom(Plot plot, PlotSet owner) {
        if (plot is not SctPlot plt) return;
        lock (form.Plot) form.Plot.Remove(plt.Plot);
        owner.Remove(plot);
    }

    public override async Task ChangePlotVisibilityAsync(Plot plot, bool isVisible)
        => await Task.Run(() => ChangeVisibility(plot, isVisible));

    public override async Task ChangePlotSetVisibilityAsync(PlotSet set, bool isVisible)
        => await Task.Run(() => Parallel.ForEach((IEnumerable<Plot>)set, plot => ChangeVisibility(plot, isVisible)));


    private void ChangeVisibility(Data data, bool isVisible) {
        if (data is not SctPlot plot) return;
        plot.ChangeVisibility(isVisible);
    }

    public override async Task ChangePlotHighlightionAsync(Plot plot) {
        if (HighlightedPlot != null)
            await Task.Run(() => ChangeHighlightion(HighlightedPlot, false));

        if (HighlightedPlot != plot) {
            HighlightedPlot = plot;
            await Task.Run(() => ChangeHighlightion(HighlightedPlot, true));
        }
        else HighlightedPlot = null;
    }

    public override async Task ChangePlotSetHighlightionAsync(PlotSet set) {
        if (HighlightedSet != null)
            await Task.Run(() => Parallel.ForEach((IEnumerable<Plot>)HighlightedSet, plot => ChangeHighlightion(plot, false)));

        if (HighlightedSet != set) {
            HighlightedSet = set;
            await Task.Run(() => Parallel.ForEach((IEnumerable<Plot>)HighlightedSet, plot => ChangeHighlightion(plot, true)));
        }
        else HighlightedSet = null;
    }


    private void ChangeHighlightion(Plot plot, bool isHighlighted) {
        if (plot is not SctPlot sct) return;
        sct.ChangeHighlightion(isHighlighted);
    }

    public override void Refresh() {
        form.Refresh();
        form.Plot.Axes.AutoScaleX();
        form.Plot.Axes.AutoScaleY();
    }

    public override void Clear() {
        storage.Clear();
        HighlightedPlot = null;
        HighlightedSet = null;
        form.Plot.Clear();
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
}
