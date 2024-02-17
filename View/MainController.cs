using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;
using ScottPlot.WinForms;
using View.Controllers;

namespace View;
public class MainController {
    private WindowsDirectoryController dirController;
    private WindowsDataController dataController;
    private ScottPlotController plotController;

    public event Action? OnDataChanged;
    public event Action? OnPlotChanged;
    public event Action? OnRootChanged;

    public MainController(FormsPlot plot) {
        //var pathToDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //dirController= new WorkingDirectory(pathToDesktop);
        //dirController= new WorkingDirectory("D:\\Study\\Chemfuck\\Lab\\MixturesData");
        dirController = new WindowsDirectoryController("d:\\Study\\Chemfuck\\Lab\\MixturesData\\single-components\\sugar-our\\");
        dataController = new WindowsDataController(new WindowsWriter());
        plotController = new ScottPlotController(plot);
    }

    #region PlotMethods

    public async Task PlotAddDataToDefault(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Left && GetClickData(sender, out Data data)) {
            await plotController.AddDataPlotAsync(data);
            plotController.Refresh();
            OnPlotChanged?.Invoke();
        }
    }

    public async Task PlotAddDataSetToDefault(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Left && GetClickSet(sender, out DataSet set)) {
            await plotController.AddDataSetPlotAsync(set, true);
            plotController.Refresh();
            OnPlotChanged?.Invoke();
        }
    }

    public async Task ContextDataPlot(object? sender, EventArgs e) {
        plotController.Clear();
        GetContextData(sender, out Data data);
        await plotController.AddDataPlotAsync(data);
        plotController.Refresh();
        OnPlotChanged?.Invoke();
    }

    public async Task ContextDataSetAddToPlot(object? sender, EventArgs e) {
        GetContextSet(sender, out DataSet set);
        await plotController.AddDataSetPlotAsync(set, false);
        plotController.Refresh();
        OnPlotChanged?.Invoke();
    }

    public async Task ContextDataSetPlot(object? sender, EventArgs e) {
        plotController.Clear();
        await ContextDataSetAddToPlot(sender, e);
    }

    public async Task ContextPlotDelete(object? sender, EventArgs e) {
        GetContextData(sender, out Data plot);
        GetContextParentSet(sender, out DataSet set);
        await plotController.RemovePlotAsync((Plot)plot, (PlotSet)set);
        plotController.Refresh();
        OnPlotChanged?.Invoke();
    }

    public async Task ContextPlotSetDelete(object? sender, EventArgs e) {
        GetContextSet(sender, out DataSet set);
        await plotController.RemovePlotSetAsync((PlotSet)set);
        plotController.Refresh();
        OnPlotChanged?.Invoke();
    }

    public async Task ChangePlotVisibility(object? sender, TreeViewEventArgs e) {
        if (e.Node?.Tag is Plot plot) {
            await plotController.ChangePlotVisibilityAsync(plot, e.Node.Checked);
            plotController.Refresh();
        }
    }

    public async Task ChangePlotSetVisibility(object? sender, TreeViewEventArgs e) {
        if (e.Node?.Tag is PlotSet set) {
            await plotController.ChangePlotSetVisibilityAsync(set, e.Node.Checked);
            plotController.Refresh();
            OnPlotChanged?.Invoke();
        }
    }

    public async Task PlotChangePlotHighlightion(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Node.Tag is Plot plot && e.Node.Checked) {
            await plotController.ChangePlotHighlightionAsync(plot);
            plotController.Refresh();
        }
    }

    public async Task PlotChangePlotSetHighlightion(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Node?.Tag is PlotSet set && e.Node.Checked) {
            await plotController.ChangePlotSetHighlightionAsync(set);
            plotController.Refresh();
        }
    }

    public void PlotClear() {
        plotController.Clear();
        plotController.Refresh();
        OnPlotChanged?.Invoke();

    }

    public void PlotRefresh() {

    }

    public IEnumerable<TreeNode> PlotGetTree() => plotController.GetTree();

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

    public IEnumerable<TreeNode> DataGetTree() => dataController.GetTree();

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

    public IEnumerable<TreeNode> RootGetTree() => dirController.GetTree();

    #endregion

    private TreeNode GetContextTreeNode(object? sender) {
        var menuItem = sender as ToolStripMenuItem;
        var contextMenuStrip = menuItem.Owner as ContextMenuStrip;
        var treeView = contextMenuStrip.SourceControl as TreeView;
        return treeView.SelectedNode;
    }

    private bool GetClickData(object? sender, out Data data) {
        var treeView = sender as TreeView;
        data = treeView.SelectedNode.Tag as Data;
        return data != null;
    }

    private bool GetClickSet(object? sender, out DataSet set) {
        var treeView = sender as TreeView;
        set = treeView.SelectedNode.Tag as DataSet;
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
}
