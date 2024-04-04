using Controllers;
using DataSource.FileSource;
using Domain;
using Domain.MathHelp;
using Domain.SpectraData;
using Domain.SpectraData.ProcessingInfo;
using Scott.Data;
using Scott.GraphicsData;
using ScottPlot.WinForms;

namespace View.Controllers;

public class WinformsMainController {
    private readonly FormsPlot form;
    private readonly DialogController dialogController;
    private readonly DirectorySourceController<Spectra> spectraSource;
    private readonly DataStorageController<Spectra> spectraStorage;
    private readonly DirectoryDataWriterController<Spectra> spectraWriter;
    private readonly DirectoryDataWriterController<PeakInfoSet> peaksInfoWriter;
    private readonly DataStorageController<Spectra> spectraPlotStorage;
    private readonly ScottGraphicsController spectraGraphicsController;
    private readonly CoordrinateController coordinateController;
    private readonly SpectraProcessingController processingController;

    public event Action? OnDataChanged;
    public event Action? OnPlotChanged;
    public event Action? OnRootChanged;
    public event Action? OnPlotMouseCoordinatesChanged;

    public Point<float> PlotCoordinates => coordinateController.Coordinates;

    public WinformsMainController(FormsPlot form) {
        this.form = form;
        dialogController = new WinformsDialogController();
        var path = "D:\\Study\\Chemfuck\\Lab\\MixturesData\\laba";
        spectraSource = new(path, new SpectraFileReader(new ScottSpectraParser()));
        spectraStorage = new("Single data");
        spectraWriter = new();
        peaksInfoWriter = new();
        spectraPlotStorage = new("Single plots");
        coordinateController = new WinformsCoordinateController(form);
        ScottSpectraGraphics graphics = new(form.Plot, new ScottPlot.Palettes.Category20());
        spectraGraphicsController = new(graphics);
        processingController = new(graphics);
    }

    #region PlotControllerMethods

    public Task ContextSetAddDraw(object? sender, EventArgs e) {
        var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        spectraPlotStorage.AddSet(new DataSet<Spectra>(set.Name, set));
        spectraGraphicsController.DrawSet(set);
        form.Refresh();
        OnPlotChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextDataClearDraw(object? sender, EventArgs e) {
        spectraPlotStorage.Clear();
        spectraGraphicsController.Clear();
        ContextSetAddDraw(sender, e);
        return Task.CompletedTask;
    }

    public Task DataAddDrawToDefault(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is not MouseButtons.Left || e.Node.Tag is not Spectra spectra) return Task.CompletedTask;
        spectraPlotStorage.AddDataToDefault(spectra);
        spectraGraphicsController.Draw(spectra);
        form.Refresh();
        OnPlotChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextClearDataDrawToDefault(object? sender, EventArgs e) {
        var spectra = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
        spectraPlotStorage.Clear();
        spectraPlotStorage.AddDataToDefault(spectra);
        spectraGraphicsController.Clear();
        spectraGraphicsController.Draw(spectra);
        form.Refresh();
        OnPlotChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ChangeSetVisibility(object? sender, TreeViewEventArgs e) {
        if (e.Node?.Tag is DataSet<Spectra> set) {
            spectraGraphicsController.SetChangeVisibility(set, e.Node.Checked);
            form.Refresh();
            OnPlotChanged?.Invoke();
        }
        return Task.CompletedTask;
    }

    public Task ChangeDataVisibility(object? sender, TreeViewEventArgs e) {
        if (e.Node?.Tag is Spectra spectra) {
            spectraGraphicsController.ChangeVisibility(spectra, e.Node.Checked);
            form.Refresh();
        }
        return Task.CompletedTask;
    }

    public Task ContextSetHighlight(object? sender, EventArgs e) {
        var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        spectraGraphicsController.SetChangeHighlightion(set);
        form.Refresh();
        return Task.CompletedTask;
    }

    public Task DataHighlight(object? sender, TreeNodeMouseClickEventArgs e) {
        var node = WinformsTreeViewHelpers.GetClickTreeNode(sender);
        if (node.Tag is Spectra spectra && node.Checked) {
            spectraGraphicsController.ChangeHighlightion(spectra);
            form.Refresh();
        }
        return Task.CompletedTask;
    }

    public Task ContextSetDelete(object? sender, EventArgs e) {
        var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        spectraPlotStorage.DeleteSet(set);
        spectraGraphicsController.EraseSet(set);
        form.Refresh();
        OnPlotChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextSpectraDelete(object? sender, EventArgs e) {
        var spectra = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
        var owner = WinformsTreeViewHelpers.GetContextParentSet<Spectra>(sender);
        spectraPlotStorage.DeleteData(owner, spectra);
        spectraGraphicsController.Erase(spectra);
        form.Refresh();
        OnPlotChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextSetPeaksProcess(object? sender, EventArgs e) {
        var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        var fullname = dialogController.SelectFullNameInDialog(set.Name, ".txt");
        if (fullname == null) return Task.CompletedTask; ;
        var info = processingController.SetProcessPeaks(set);
        peaksInfoWriter.DataWriteAs(info, fullname);
        return Task.CompletedTask;
    }

    public Task ContextDataPeaksProcess(object? sender, EventArgs e) {
        var spectra = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
        var fullname = dialogController.SelectFullNameInDialog(spectra.Name, ".txt");
        if (fullname == null) return Task.CompletedTask;
        var info = processingController.ProcessPeaks(spectra);
        peaksInfoWriter.DataWriteAs(info, fullname);
        return Task.CompletedTask;
    }

    public async Task PlotAddPeakBorders(object? sender, EventArgs e) {
        var start = await coordinateController.GetCoordinateByClick();
        var end = await coordinateController.GetCoordinateByClick();
        var border = new ScottPeakBorder(start.X, end.X);
        processingController.AddBorder(border);
        form.Refresh();
    }

    public Task PlotDeleteLastPeakBorders(object? sender, EventArgs e) {
        var last = processingController.Borders.LastOrDefault();
        if (last == default) return Task.CompletedTask;
        processingController.RemoveBorder(last);
        form.Refresh();
        return Task.CompletedTask;
    }

    public Task PlotClearPeakBorders(object? sender, EventArgs e) {
        processingController.ClearBorders();
        form.Refresh();
        return Task.CompletedTask;
    }

    public Task SetPlotCoordinates(object? sender, MouseEventArgs e) {
        var coord = form.Plot.GetCoordinates(e.X, e.Y);
        coordinateController.SetCoordinates((float)coord.X, (float)coord.Y);
        OnPlotMouseCoordinatesChanged?.Invoke();
        return Task.CompletedTask;
    }

    public void PlotClear() {
        spectraPlotStorage.Clear();
        spectraGraphicsController.Clear();
        processingController.RedrawBorders();
        form.Refresh();
        OnPlotChanged?.Invoke();
    }

    public void PlotResize() {
        spectraGraphicsController.Resize();
        form.Refresh();
    }

    public IEnumerable<TreeNode> PlotGetTree() {
        foreach (var pair in spectraPlotStorage.StorageRecords) {
            var setNode = new TreeNode {
                Text = pair.Key,
                Tag = pair.Value,
                Checked = false
            };
            foreach (var plot in pair.Value.OrderByDescending(p => p.Name)) {
                var subnode = new TreeNode {
                    Text = plot.Name,
                    Tag = plot,
                    Checked = plot.GetPlot().IsVisible
                };
                if (subnode.Checked) setNode.Checked = true;
                setNode.Nodes.Add(subnode);
            }
            if (setNode.Nodes.Count > 0)
                yield return setNode;
        }
    }

    #endregion

    #region DataControllerMethods

    public Task ContextDataSetAndSubsetsSaveAsESPAsync(object? sender, EventArgs e) {
        var path = dialogController.SelectPathInDialog();
        if (path is null) return Task.CompletedTask;
        var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        var outputPath = Path.Combine(path, $"{set.Name} (converted full depth)");
        spectraWriter.SetFullDepthWriteAs(set, outputPath, ".esp");
        return Task.CompletedTask;
    }

    public Task ContextDataSetSaveAsESPAsync(object? sender, EventArgs e) {
        var path = dialogController.SelectPathInDialog();
        if (path is null) return Task.CompletedTask;
        var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        var outputPath = Path.Combine(path, $"{set.Name} (converted)");
        spectraWriter.SetOnlyWriteAs(set, outputPath, ".esp");
        return Task.CompletedTask;
    }

    public Task ContextDataSaveAsESPAsync(object? sender, EventArgs e) {
        var data = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
        var fullname = dialogController.SelectFullNameInDialog(data.Name, ".esp");
        if (fullname is null) return Task.CompletedTask;
        spectraWriter.DataWriteAs(data, fullname);
        return Task.CompletedTask;
    }

    public Task ContextSetFullDepthSubstractBaselineAsync(object? sender, EventArgs e) {
        var root = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        var substractedSet = SpectraProcessingController.SetFullDepthSubstractBaseLin(root);
        spectraStorage.AddSet(substractedSet);
        OnDataChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextSetOnlySubstractBaselineAsync(object? sender, EventArgs e) {
        var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        var substractedSet = SpectraProcessingController.SetOnlySubstractBaseline(set);
        spectraStorage.AddSet(substractedSet);
        OnDataChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextDataSubstractBaselineAsync(object? sender, EventArgs e) {
        var data = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
        var substracted = SpectraProcessingController.SubstractBaseline(data);
        spectraStorage.AddDataToDefault(substracted);
        OnDataChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ContextSetOnlyGetAverageSpectra(object? sender, EventArgs e) {
        var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        var average = SpectraProcessingController.SetGetAverageSpectra(set);
        spectraStorage.AddDataToDefault(average);
        OnDataChanged?.Invoke();
        return Task.CompletedTask;
    }

    public void ContextDataSetDelete(object? sender, EventArgs e) {
        var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
        spectraStorage.DeleteSet(set);
        OnDataChanged?.Invoke();
    }

    public void ContextDataDelete(object? sender, EventArgs e) {
        var spectra = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
        var owner = WinformsTreeViewHelpers.GetContextParentSet<Spectra>(sender);
        if (spectraStorage.DeleteData(owner, spectra))
            OnDataChanged?.Invoke();
    }

    public void DataClear() {
        spectraStorage.Clear();
        OnDataChanged?.Invoke();
    }

    public IEnumerable<TreeNode> DataGetTree() {
        foreach (var pair in spectraStorage.StorageRecords) {
            var node = new TreeNode { Text = pair.Key, Tag = pair.Value };
            ConnectDataSubnodes(node);
            yield return node;
        }
    }

    private void ConnectDataSubnodes(TreeNode node) {
        if (node.Tag is not DataSet<Spectra> set)
            throw new Exception(nameof(ConnectDataSubnodes));

        foreach (var child in set.Subsets.OrderByDescending(child => child.Name)) {
            var subnode = new TreeNode {
                Text = child.Name,
                Tag = child,
            };
            ConnectDataSubnodes(subnode);
            node.Nodes.Add(subnode);
        }

        foreach (var data in set.OrderByDescending(data => data.Name)) {
            var subnode = new TreeNode() {
                Text = data.Name,
                Tag = data,
            };
            node.Nodes.Add(subnode);
        }
    }

    #endregion

    #region DirectoryControllerMethods

    public Task RootReadWithSubdirsAsync(object? sender, EventArgs e) {
        var set = spectraSource.ReadRootFullDepth();
        spectraStorage.AddSet(set);
        OnDataChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task RootReadAsync(object? sender, EventArgs e) {
        var set = spectraSource.ReadRoot();
        spectraStorage.AddSet(set);
        OnDataChanged?.Invoke();
        return Task.CompletedTask;
    }

    public void RootSelect(object? sender, EventArgs e) {
        var selectedPath = dialogController.SelectPathInDialog();
        if (selectedPath != null && spectraSource.ChangeRoot(selectedPath))
            OnRootChanged?.Invoke();
    }

    public void RootStepBack(object? sender, EventArgs e) {
        if (spectraSource.StepBack())
            OnRootChanged?.Invoke();
    }

    public void RootRefresh(object? sender, EventArgs e) {
        OnRootChanged?.Invoke();
    }

    public Task RootDoubleClick(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Node.Tag is FileInfo file) {
            var data = spectraSource.Read(file.FullName);
            if (data != null && spectraStorage.AddDataToDefault(data))
                OnDataChanged?.Invoke();
        }
        else if (e.Node.Tag is DirectoryInfo newRoot) {
            if (spectraSource.ChangeRoot(newRoot.FullName))
                OnRootChanged?.Invoke();
        }
        return Task.CompletedTask;
    }

    public IEnumerable<TreeNode> RootGetTree() {
        foreach (var dir in spectraSource.Root.GetDirectories().OrderByDescending(d => d.Name)) {
            yield return new TreeNode {
                Text = dir.Name,
                Tag = dir,
                ImageIndex = 0
            };
        }
        foreach (var file in spectraSource.Root.GetFiles().OrderByDescending(f => f.Name)) {
            yield return new TreeNode {
                Text = file.Name,
                Tag = file,
                ImageIndex = 1
            };
        }
    }

    #endregion
}