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
        controller.OnPlotMouseCoordinatesChanged += DrawMouseCoordinates;

        //_ = BuildTreeAsync(rootTree, controller.RootGetTree);
        //_ = BuildTreeAsync(dataTree, controller.DataGetTree);

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
        dataContextDataSetSaveAs.Click += async (sender, e) => await controller.ContextDataSetSaveAsESPAsync(sender, e);
        dataContextDataSetAndSubsetsSaveAs.Click += async (sender, e) => await controller.ContextDataSetAndSubsetsSaveAsESPAsync(sender, e);
        dataContextDataSetDelete.Click += (sender, e) => controller.ContextDataSetDelete(sender, e);
        dataContextDataSetPlot.Click += async (sender, e) => await controller.ContextDataSetPlot(sender, e);
        dataContextDataSetAddToPlot.Click += async (sender, e) => await controller.ContextDataSetAddToPlot(sender, e);
        dataContextDataSetSubstactBaseline.Click += async (sender, e) => await controller.ContextDataSetSubstractBaseline(sender, e);
        dataContextDataSetAndSubsetsSubstractBaseline.Click += async (sender, e) => await controller.ContextDataSetAndSubsetsSubstractBaseline(sender, e);

        dataContextDataSave.Click += async (sender, e) => await controller.ContextDataSaveAsESPAsync(sender, e);
        dataContextDataDelete.Click += (sender, e) => controller.ContextDataDelete(sender, e);
        dataContextDataPlot.Click += async (sender, e) => await controller.ContextDataPlot(sender, e);
        dataContextDataSubstractBaseline.Click += async (sender, e) => await controller.ContextDataSubstractBaseline(sender, e);

        plotSetMenu.Tag = plotTree;
        plotMenu.Tag = plotTree;
        plotView.MouseMove += async (sender, e) => await controller.SetPlotCoordinates(sender, e);
        plotTree.NodeMouseClick += TreeNodeClickSelect;
        plotTree.NodeMouseClick += PlotDrawContextMenu;
        plotTree.NodeMouseClick += PlotSetDrawContextMenu;
        plotTree.NodeMouseDoubleClick += TreeNodeClickSelect;
        plotTree.NodeMouseDoubleClick += async (sender, e) => await controller.PlotChangePlotHighlightion(sender, e);
        plotButtonClear.Click += (sender, e) => controller.PlotClear();
        plotButtonResize.Click += (sender, e) => controller.PlotResize();
        plotContextPlotSetHighlight.Click += async (sender, e) => await controller.ContextPlotSetHighlight(sender, e);
        plotContextPlotSetDelete.Click += async (sender, e) => await controller.ContextPlotSetDelete(sender, e);
        plotContextPlotDelete.Click += async (sender, e) => await controller.ContextPlotDelete(sender, e);
        plotTree.AfterCheck += async (sender, e) => await controller.ChangePlotSetVisibility(sender, e);
        plotTree.AfterCheck += async (sender, e) => await controller.ChangePlotVisibility(sender, e);

        plotButtonAddPeak.Click += async (sender, e) => await controller.PlotAddPeak(sender, e);
        plotButtonDeleteLastPeak.Click += async (sender, e) => await controller.PlotDeleteLastPeak(sender, e);
        plotButtonClearPeaks.Click += async (sender, e) => await controller.PlotClearPeaks(sender, e);

        plotContextPlotSetPeaksProcess.Click += async (sender, e) => await controller.ContextPlotSetPeaksProcess(sender, e);
        plotContextPlotPeaksProcess.Click += async (sender, e) => await controller.ContextPlotPeaksProcess(sender, e);
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
