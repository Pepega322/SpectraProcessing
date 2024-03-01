using Model.Controllers;
using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;
using Model.MathHelper;
using ScottPlot.WinForms;
using View.Controllers;

namespace View;
public class MainController {
    private RootController dirController;
    private DataController dataController;
    private PlotController plotController;

    public event Action? OnDataChanged;
    public event Action? OnPlotChanged;
    public event Action? OnRootChanged;
    public event Action? OnPlotMouseCoordinatesChanged;

    public Point<float> PlotCoordinates => plotController.Coordinates;

    public MainController(FormsPlot plot) {
        //var pathToDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //dirController = new WindowsDirectoryController(pathToDesktop);
        //dirController= new WindowsDirectoryController("D:\\Study\\Chemfuck\\Lab\\MixturesData");
        dirController = new WindowsDirectoryController("d:\\Study\\Chemfuck\\Lab\\MixturesData\\TestSpectras\\");
        dataController = new WindowsDataController(new WindowsWriter());
        plotController = new ScottPlotController(plot);
    }

    #region PlotControllerMethods

    public async Task ContextDataSetAddToPlot(object? sender, EventArgs e) {
        if (GetContextSet(sender, out DataSet set)) {
            await plotController.AddDataSetPlotAsync(set, false);
            plotController.Refresh();
            OnPlotChanged?.Invoke();
        }
        else throw new Exception();
    }

    public async Task ContextDataSetPlot(object? sender, EventArgs e) {
        plotController.Clear();
        await ContextDataSetAddToPlot(sender, e);
    }

    public async Task PlotAddDataToDefault(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is not MouseButtons.Left) return;
        if (GetClickData(sender, out Data data)) {
            await plotController.AddDataPlotAsync(data);
            plotController.Refresh();
            OnPlotChanged?.Invoke();
        }
    }

    public async Task ContextDataPlot(object? sender, EventArgs e) {
        if (GetContextData(sender, out Data data)) {
            plotController.Clear();
            await plotController.AddDataPlotAsync(data);
            plotController.Refresh();
            OnPlotChanged?.Invoke();

        }
        else throw new Exception();
    }

    public async Task ChangePlotSetVisibility(object? sender, TreeViewEventArgs e) {
        if (e.Node?.Tag is PlotSet set) {
            await plotController.ChangePlotSetVisibilityAsync(set, e.Node.Checked);
            plotController.Refresh();
            OnPlotChanged?.Invoke();
        }
    }

    public async Task ContextPlotSetHighlight(object? sender, EventArgs e) {
        if (GetContextSet(sender, out DataSet set)) {
            await plotController.ChangePlotSetHighlightionAsync((PlotSet)set);
            plotController.Refresh();
        }
        else throw new Exception();
    }

    public async Task ContextPlotSetDelete(object? sender, EventArgs e) {
        if (GetContextSet(sender, out DataSet set)) {
            await plotController.RemovePlotSetAsync((PlotSet)set);
            plotController.Refresh();
            OnPlotChanged?.Invoke();
        }
        else throw new Exception();
    }

    public async Task ContextPlotSetPeaksProcess(object? sender, EventArgs e) {
        if (GetContextSet(sender, out DataSet set)) {
            var info = await plotController.ProcessPlotSet((PlotSet)set);
            var path = dirController.SelectPathInDialog();
            if (path == null) return;
            await dataController.WriteDataAsAsync(info, path, ".txt");
        }
        else throw new Exception();
    }

    public async Task ChangePlotVisibility(object? sender, TreeViewEventArgs e) {
        if (e.Node?.Tag is Plot plot) {
            await plotController.ChangePlotVisibilityAsync(plot, e.Node.Checked);
            plotController.Refresh();
        }
    }

    public async Task PlotChangePlotHighlightion(object? sender, TreeNodeMouseClickEventArgs e) {
        var node = GetClickTreeNode(sender);
        if (node.Tag is Plot plot && node.Checked) {
            await plotController.ChangePlotHighlightionAsync(plot);
            plotController.Refresh();
        }
    }

    public async Task ContextPlotDelete(object? sender, EventArgs e) {
        if (GetContextData(sender, out Data plot) && GetContextParentSet(sender, out DataSet set)) {
            await plotController.RemovePlotAsync((Plot)plot, (PlotSet)set);
            plotController.Refresh();
            OnPlotChanged?.Invoke();
        }
        else throw new Exception();
    }

    public async Task ContextPlotPeaksProcess(object? sender, EventArgs e) {
        if (GetContextData(sender, out Data data)) {
            var info = await plotController.ProcessPlot((Plot)data);
            var path = dirController.SelectPathInDialog();
            if (path == null) return;
            await dataController.WriteDataAsAsync(info, path, ".txt");
        }
        else throw new Exception();
    }

    public async Task PlotAddPeak(object? sender, EventArgs e) {
        bool result = await plotController.AddBorder();
        if (result) plotController.Refresh();
    }

    public async Task PlotDeleteLastPeak(object? sender, EventArgs e) {
        bool result = await plotController.DeleteLastBorder();
        if (result) plotController.Refresh();
    }

    public async Task PlotClearPeaks(object? sender, EventArgs e) {
        bool result = await plotController.ClearBorders();
        if (result) plotController.Refresh();
    }

    public async Task SetPlotCoordinates(object? sender, MouseEventArgs e) {
        await plotController.SetCoordinates(e.X, e.Y);
        OnPlotMouseCoordinatesChanged?.Invoke();
    }

    public void PlotClear() {
        plotController.Clear();
        plotController.Refresh();
        OnPlotChanged?.Invoke();
    }

    public void PlotResize() {
        plotController.Resize();
        plotController.Refresh();
    }

    public IEnumerable<TreeNode> PlotGetTree() => ((ITree)plotController).GetTree();

    #endregion

    #region DataControllerMethods

    public async Task ContextDataSetAndSubsetsSaveAsESPAsync(object? sender, EventArgs e) {
        var path = dirController.SelectPathInDialog();
        if (path is null) return;
        GetContextSet(sender, out DataSet set);
        var outputPath = Path.Combine(path, $"{set.Name} (converted all)");
        await dataController.WriteSetAsAsync(set, outputPath, ".esp", true);
    }

    public async Task ContextDataSetSaveAsESPAsync(object? sender, EventArgs e) {
        var path = dirController.SelectPathInDialog();
        if (path is null) return;
        GetContextSet(sender, out DataSet set);
        var outputPath = Path.Combine(path, $"{set.Name} (converted only this)");
        await dataController.WriteSetAsAsync(set, outputPath, ".esp", false);
    }

    public async Task ContextDataSaveAsESPAsync(object? sender, EventArgs e) {
        var path = dirController.SelectPathInDialog();
        if (path is null) return;
        GetContextData(sender, out Data data);
        await dataController.WriteDataAsAsync(data, path, ".esp");
    }

    public async Task ContextDataSetAndSubsetsSubstractBaseline(object? sender, EventArgs e) {
        GetContextSet(sender, out DataSet set);
        await dataController.SubstractBaselineForSetAsync(set, true);
        OnDataChanged?.Invoke();
    }

    public async Task ContextDataSetSubstractBaseline(object? sender, EventArgs e) {
        GetContextSet(sender, out DataSet set);
        await dataController.SubstractBaselineForSetAsync(set, false);
        OnDataChanged?.Invoke();
    }

    public async Task ContextDataSubstractBaseline(object? sender, EventArgs e) {
        GetContextData(sender, out Data data);
        GetContextParentSet(sender, out DataSet set);
        await dataController.SubstractBaselineForDataAsync(data);
        OnDataChanged?.Invoke();
    }

    public void ContextDataSetDelete(object? sender, EventArgs e) {
        GetContextSet(sender, out DataSet set);
        if (dataController.DeleteSet(set))
            OnDataChanged?.Invoke();
    }

    public void ContextDataDelete(object? sender, EventArgs e) {
        GetContextData(sender, out Data data);
        GetContextParentSet(sender, out DataSet set);
        if (dataController.DeleteData(set, data))
            OnDataChanged?.Invoke();
    }

    public void DataClear() {
        dataController.Clear();
        OnDataChanged?.Invoke();
    }

    public IEnumerable<TreeNode> DataGetTree() => ((ITree)dataController).GetTree();

    #endregion

    #region DirectoryControllerMethods

    public async Task RootReadWithSubdirsAsync(object? sender, EventArgs e) {
        var set = await dirController.ReadRoot(true);
        if (dataController.AddSet(set.Name, set))
            OnDataChanged?.Invoke();
    }

    public async Task RootReadAsync(object? sender, EventArgs e) {
        var set = await dirController.ReadRoot();
        if (dataController.AddSet(set.Name, set))
            OnDataChanged?.Invoke();
    }

    public void RootSelect(object? sender, EventArgs e) {
        var selectedPath = dirController.SelectPathInDialog();
        if (selectedPath != null && dirController.ChangeRoot(selectedPath))
            OnRootChanged?.Invoke();
    }

    public void RootStepBack(object? sender, EventArgs e) {
        if (dirController.StepBack())
            OnRootChanged?.Invoke();
    }

    public void RootRefresh(object? sender, EventArgs e) {
        OnRootChanged?.Invoke();
    }

    public async void RootDoubleClick(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Node.Tag is FileInfo file) {
            var data = await dirController.ReadData(file.FullName);
            if (dataController.AddDataToDefaultSet(data))
                OnDataChanged?.Invoke();
        }

        if (e.Node.Tag is DirectoryInfo newRoot) {
            if (dirController.ChangeRoot(newRoot.FullName))
                OnRootChanged?.Invoke();
        }
    }

    public IEnumerable<TreeNode> RootGetTree() => ((ITree)dirController).GetTree();

    #endregion

    #region SupportWinformMethods

    private TreeNode GetContextTreeNode(object? sender) {
        var item = sender as ToolStripDropDownItem;
        var contextMenu = item.Owner as ContextMenuStrip;
        while (contextMenu == null) {
            var t = item.Owner as ToolStripDropDownMenu;
            contextMenu = t.OwnerItem.Owner as ContextMenuStrip;
            item = t.OwnerItem as ToolStripDropDownItem;
        }
        var treeView = contextMenu.Tag as TreeView;
        return treeView.SelectedNode;
    }

    public TreeNode GetClickTreeNode(object? sender) {
        var treeView = sender as TreeView;
        if (treeView.SelectedNode != null)
            return treeView.SelectedNode;
        else throw new Exception();
    }

    private bool GetClickData(object? sender, out Data data) {
        var node = GetClickTreeNode(sender);
        data = node.Tag as Data;
        return data != null;
    }

    private bool GetClickSet(object? sender, out DataSet set) {
        var node = GetClickTreeNode(sender);
        set = node.Tag as DataSet;
        return set != null;
    }

    private bool GetContextData(object? sender, out Data data) {
        var node = GetContextTreeNode(sender);
        data = node.Tag as Data;
        return data != null;
    }

    private bool GetContextSet(object? sender, out DataSet set) {
        var node = GetContextTreeNode(sender);
        set = node.Tag as DataSet;
        return set != null;
    }

    private bool GetContextParentSet(object? sender, out DataSet set) {
        var node = GetContextTreeNode(sender);
        set = node.Parent.Tag as DataSet;
        return set != null;
    }

    #endregion
}
