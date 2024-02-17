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

        _ = BuildTreeAsync(rootTree, controller.RootGetTree);
        _ = BuildTreeAsync(dataTree, controller.DataGetTree);



        rootTree.NodeMouseClick += TreeNodeSelect;
        rootButtonSelect.Click += (sender, e) => controller.RootSelect(sender, e);
        rootButtonStepBack.Click += (sender, e) => controller.RootStepBack(sender, e);
        rootButtonRefresh.Click += (sender, e) => controller.RootRefresh(sender, e);
        rootButtonRead.Click += async (sender, e) => await controller.RootReadAsync(sender, e);
        rootButtonReadWithSubdirs.Click += async (sender, e) => await controller.RootReadWithSubdirsAsync(sender, e);
        rootTree.NodeMouseDoubleClick += (sender, e) => controller.RootDoubleClick(sender, e);

        dataTree.NodeMouseClick += TreeNodeSelect;
        dataTree.NodeMouseClick += DataDrawContextMenu;
        dataTree.NodeMouseClick += DataSetDrawContextMenu;
        dataTree.NodeMouseDoubleClick += async (sender, e) => await controller.PlotAddDataSetToDefault(sender, e);
        dataTree.NodeMouseDoubleClick += async (sender, e) => await controller.PlotAddDataToDefault(sender, e);

        dataButtonClear.Click += (sender, e) => controller.DataClear();
        dataButtonContextDataSetSaveAs.Click += async (sender, e) => await controller.ContextDataSetSaveAsESPAsync(sender, e);
        dataButtonContextDataSetAndSubsetsSaveAs.Click += async (sender, e) => await controller.ContextDataSetAndSubsetsSaveAsESPAsync(sender, e);
        dataButtonContextDataSetDelete.Click += (sender, e) => controller.ContextDataSetDelete(sender, e);
        dataButtonContextDataSetPlot.Click += async (sender, e) => await controller.ContextDataSetPlot(sender, e);
        dataButtonContextDataSetAddToPlot.Click += async (sender, e) => await controller.ContextDataSetAddToPlot(sender, e);
        //dataButtonContextDataSetSubstractBaseline += 
        dataButtonContextDataSave.Click += async (sender, e) => await controller.ContextDataSaveAsESPAsync(sender, e);
        dataButtonContextDataDelete.Click += (sender, e) => controller.ContextDataDelete(sender, e);
        dataButtonContextDataPlot.Click += async (sender, e) => await controller.ContextDataPlot(sender, e);
        //dataButtonContextDataSubstractBaseline += 

        //plotView.MouseMove += (sender, e) => DrawMouseCoordinates();
        plotTree.NodeMouseClick += TreeNodeSelect;
        plotTree.NodeMouseClick += PlotDrawContextMenu;
        plotTree.NodeMouseClick += PlotSetDrawContextMenu;
        plotTree.NodeMouseDoubleClick += async (sender, e) => await controller.PlotChangePlotHighlightion(sender, e);
        plotTree.NodeMouseDoubleClick += async (sender, e) => await controller.PlotChangePlotSetHighlightion(sender, e);
        plotButtonClear.Click += (sender, e) => controller.PlotClear();
        plotButtonContextPlotSetDelete.Click += async (sender, e) => await controller.ContextPlotSetDelete(sender, e);
        plotButtonContextPlotDelete.Click += async (sender, e) => await controller.ContextPlotDelete(sender, e);
        plotTree.AfterCheck += async (sender, e) => await controller.ChangePlotSetVisibility(sender, e);
        plotTree.AfterCheck += async (sender, e) => await controller.ChangePlotVisibility(sender, e);

    }

    private void TreeNodeSelect(object? sender, TreeNodeMouseClickEventArgs e) {
        var treeView = sender as TreeView;
        if (treeView != null) treeView.SelectedNode = e.Node;
        else throw new Exception(nameof(TreeNodeSelect));
    }

    private void PlotSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Right && e.Node.Tag is PlotSet set) {
            e.Node.ContextMenuStrip = plotSetMenu;
        }
    }

    private void PlotDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Right && e.Node.Tag is Plot plot) {
            if (e.Node.Parent.Tag is not PlotSet)
                throw new Exception("PlotDrawContextMenu Method works wrong");
            e.Node.ContextMenuStrip = plotMenu;
        }
    }

    private void DataSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Right && e.Node.Tag is DataSet set) {
            e.Node.ContextMenuStrip = dataSetMenu;
        }
    }

    private void DataDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e) {
        if (e.Button is MouseButtons.Right && e.Node.Tag is Data data) {
            if (e.Node.Parent.Tag is not TreeDataSetNode)
                throw new Exception("DataDrawContextMenu Method works wrong");
            e.Node.ContextMenuStrip = dataMenu;
        }
    }

    private async Task BuildTreeAsync(TreeView tree, Func<IEnumerable<TreeNode>> nodeSource) {
        tree.Nodes.Clear();
        tree.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        tree.Nodes.AddRange(nodes);
        tree.EndUpdate();
    }
}
