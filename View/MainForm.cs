using Model.DataFormats;
using Model.DataStorages;

namespace View;

public partial class MainForm : Form {
    private MainController controller;

    public MainForm() {
        InitializeComponent();
        controller = new(plotView);
        controller.OnRootChanged += async () => await BuildTreeAsync(rootTree, controller.RootGetTree);
        controller.OnDataChanged += async () => await BuildTreeAsync(dataTree, controller.DataGetTree);
        controller.OnPlotChanged += async () => await BuildTreeAsync(plotTree, controller.PlotGetTree);
        controller.OnMousePlotCoordinatesChanged += DrawMouseCoordinates;

        _ = BuildTreeAsync(rootTree, controller.RootGetTree);
        _ = BuildTreeAsync(dataTree, controller.DataGetTree);

        rootTree.NodeMouseClick += TreeNodeClickSelect;
        rootTree.NodeMouseDoubleClick += (sender, e) => controller.RootDoubleClick(sender, e);
        rootButtonSelect.Click += (sender, e) => controller.RootSelect(sender, e);
        rootButtonStepBack.Click += (sender, e) => controller.RootStepBack(sender, e);
        rootButtonRefresh.Click += (sender, e) => controller.RootRefresh(sender, e);
        rootButtonRead.Click += async (sender, e) => await controller.RootReadAsync(sender, e);
        rootButtonReadWithSubdirs.Click += async (sender, e) => await controller.RootReadWithSubdirsAsync(sender, e);

        dataSetMenu.Tag = dataTree;
        dataMenu.Tag = dataTree;
        dataTree.NodeMouseClick += TreeNodeClickSelect;
        dataTree.NodeMouseClick += DataDrawContextMenu;
        dataTree.NodeMouseClick += DataSetDrawContextMenu;
        dataTree.NodeMouseDoubleClick += async (sender, e) => await controller.PlotAddDataToDefault(sender, e);

        dataButtonClear.Click += (sender, e) => controller.DataClear();
        dataButtonContextDataSetSaveAs.Click += async (sender, e) => await controller.ContextDataSetSaveAsESPAsync(sender, e);
        dataButtonContextDataSetAndSubsetsSaveAs.Click += async (sender, e) => await controller.ContextDataSetAndSubsetsSaveAsESPAsync(sender, e);
        dataButtonContextDataSetDelete.Click += (sender, e) => controller.ContextDataSetDelete(sender, e);
        dataButtonContextDataSetPlot.Click += async (sender, e) => await controller.ContextDataSetPlot(sender, e);
        dataButtonContextDataSetAddToPlot.Click += async (sender, e) => await controller.ContextDataSetAddToPlot(sender, e);
        dataButtonContextDataSetSubstactBaseline.Click += async (sender, e) => await controller.ContextDataSetSubstractBaseline(sender, e);
        dataButtonContextDataSetAndSubsetsSubstractBaseline.Click += async (sender, e) => await controller.ContextDataSetAndSubsetsSubstractBaseline(sender, e);

        dataButtonContextDataSave.Click += async (sender, e) => await controller.ContextDataSaveAsESPAsync(sender, e);
        dataButtonContextDataDelete.Click += (sender, e) => controller.ContextDataDelete(sender, e);
        dataButtonContextDataPlot.Click += async (sender, e) => await controller.ContextDataPlot(sender, e);
        dataButtonContextDataSubstractBaseline.Click += async (sender, e) => await controller.ContextDataSubstractBaseline(sender, e);

        plotSetMenu.Tag = plotTree;
        plotMenu.Tag = plotTree;
        plotView.MouseMove += async (sender, e) => await controller.SetPlotCoordinates(sender, e);
        plotTree.NodeMouseClick += TreeNodeClickSelect;
        plotTree.NodeMouseClick += PlotDrawContextMenu;
        plotTree.NodeMouseClick += PlotSetDrawContextMenu;
        plotTree.NodeMouseDoubleClick += TreeNodeClickSelect;
        plotTree.NodeMouseDoubleClick += async (sender, e) => await controller.PlotChangePlotHighlightion(sender, e);
        plotButtonClear.Click += (sender, e) => controller.PlotClear();
        plotButtonContextPlotSetHighlight.Click += async (sender, e) => await controller.ContextPlotSetHighlight(sender, e);
        plotButtonContextPlotSetDelete.Click += async (sender, e) => await controller.ContextPlotSetDelete(sender, e);
        plotButtonContextPlotSetPeakSelect.Click += async (sender, e) => await controller.ContextPlotSetPeakSelect(sender, e);
        plotButtonContextPlotDelete.Click += async (sender, e) => await controller.ContextPlotDelete(sender, e);
        plotButtonContextPlotPeakSelect.Click += async (sender, e) => await controller.ContextPlotPeakSelect(sender, e);
        plotTree.AfterCheck += async (sender, e) => await controller.ChangePlotSetVisibility(sender, e);
        plotTree.AfterCheck += async (sender, e) => await controller.ChangePlotVisibility(sender, e);

    }

    private void DrawMouseCoordinates() {
        var point = controller.PlotCoordinates;
        mouseCoordinatesBox.Text = $"X:{Math.Round(point.X, 2)}\tY:{Math.Round(point.Y, 2)}";
    }

    private void TreeNodeClickSelect(object? sender, TreeNodeMouseClickEventArgs e) {
        var treeView = sender as TreeView;
        if (treeView != null) treeView.SelectedNode = e.Node;
        else throw new Exception(nameof(TreeNodeClickSelect));
    }

    private void PlotSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Right && e.Node.Tag is PlotSet) {
            e.Node.ContextMenuStrip = plotSetMenu;
        }
    }

    private void PlotDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Right && e.Node.Tag is Plot)
            e.Node.ContextMenuStrip = plotMenu;
    }

    private void DataSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Right && e.Node.Tag is DataSet)
            e.Node.ContextMenuStrip = dataSetMenu;
    }

    private void DataDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Right && e.Node.Tag is Data)
            e.Node.ContextMenuStrip = dataMenu;
    }

    private async Task BuildTreeAsync(TreeView tree, Func<IEnumerable<TreeNode>> nodeSource) {
        tree.Nodes.Clear();
        tree.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        tree.Nodes.AddRange(nodes);
        tree.EndUpdate();
    }
}
