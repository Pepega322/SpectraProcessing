using Domain;
using Domain.SpectraData;
using WinformsApp.Controllers;

namespace WinformsApp;

public partial class MainForm : Form
{
    private readonly WinformsMainController controller;

    public MainForm()
    {
        InitializeComponent();
        controller = new(plotView);
        controller.OnRootChanged += async () => await BuildTreeAsync(rootTree, controller.RootGetTree);
        controller.OnDataChanged += async () => await BuildTreeAsync(datatorageTree, controller.DataGetTree);
        controller.OnPlotChanged += async () => await BuildTreeAsync(plotStorageTree, controller.PlotGetTree);
        controller.OnPlotMouseCoordinatesChanged += DrawMouseCoordinates;

        _ = BuildTreeAsync(rootTree, controller.RootGetTree);

        rootTree.NodeMouseClick += TreeNodeClickSelect;
        rootTree.NodeMouseDoubleClick += async (sender, e) => await controller.RootDoubleClick(sender, e);
        rootButtonSelect.Click += (sender, e) => controller.RootSelect(sender, e);
        rootButtonStepBack.Click += (sender, e) => controller.RootStepBack(sender, e);
        rootButtonRefresh.Click += (sender, e) => controller.RootRefresh(sender, e);
        rootButtonRead.Click += async (sender, e) => await controller.RootReadAsync(sender, e);
        rootButtonReadWithSubdirs.Click += async (sender, e) => await controller.RootReadWithSubdirsAsync(sender, e);

        dataSetMenu.Tag = datatorageTree;
        dataMenu.Tag = datatorageTree;
        datatorageTree.NodeMouseClick += TreeNodeClickSelect;
        datatorageTree.NodeMouseClick += DataDrawContextMenu;
        datatorageTree.NodeMouseClick += DataSetDrawContextMenu;
        datatorageTree.NodeMouseDoubleClick += async (sender, e) => await controller.DataAddDrawToDefault(sender, e);

        dataButtonClear.Click += (_, _) => controller.DataClear();
        dataContextDataSetSaveAs.Click += async (sender, e) => await controller.ContextDataSetSaveAsEspAsync(sender, e);
        dataContextDataSetAndSubsetsSaveAs.Click += async (sender, e) =>
            await controller.ContextDataSetAndSubsetsSaveAsEspAsync(sender, e);
        dataContextDataSetDelete.Click += (sender, e) => controller.ContextDataSetDelete(sender, e);
        dataContextDataSetPlot.Click += async (sender, e) => await controller.ContextDataClearDraw(sender, e);
        dataContextDataSetAddToPlot.Click += async (sender, e) => await controller.ContextSetAddDraw(sender, e);
        dataContextDataSetSubstactBaseline.Click += async (sender, e) =>
            await controller.ContextSetOnlySubstractBaselineAsync(sender, e);
        dataContextDataSetAndSubsetsSubstractBaseline.Click += async (sender, e) =>
            await controller.ContextSetFullDepthSubstractBaselineAsync(sender, e);
        dataContextDataSetGetAverageSpectra.Click +=
            async (sender, e) => await controller.ContextSetOnlyGetAverageSpectra(sender, e);

        dataContextDataSave.Click += async (sender, e) => await controller.ContextDataSaveAsEspAsync(sender, e);
        dataContextDataDelete.Click += (sender, e) => controller.ContextDataDelete(sender, e);
        dataContextDataPlot.Click += async (sender, e) => await controller.ContextClearDataDrawToDefault(sender, e);
        dataContextDataSubstractBaseline.Click +=
            async (sender, e) => await controller.ContextDataSubstractBaselineAsync(sender, e);

        plotSetMenu.Tag = plotStorageTree;
        plotMenu.Tag = plotStorageTree;
        plotView.MouseMove += (sender, e) => controller.SetPlotCoordinates(sender, e);
        plotStorageTree.NodeMouseClick += TreeNodeClickSelect;
        plotStorageTree.NodeMouseClick += PlotDrawContextMenu;
        plotStorageTree.NodeMouseClick += PlotSetDrawContextMenu;
        plotStorageTree.NodeMouseDoubleClick += TreeNodeClickSelect;
        plotStorageTree.NodeMouseDoubleClick += async (sender, e) => await controller.DataHighlight(sender, e);
        plotStorageTree.AfterCheck += async (sender, e) => await controller.ChangeSetVisibility(sender, e);
        plotStorageTree.AfterCheck += async (sender, e) => await controller.ChangeDataVisibility(sender, e);
        plotButtonClear.Click += (_, _) => controller.PlotClear();
        plotButtonResize.Click += (_, _) => controller.PlotResize();
        plotContextPlotSetHighlight.Click += async (sender, e) => await controller.ContextSetHighlight(sender, e);
        plotContextPlotSetDelete.Click += async (sender, e) => await controller.ContextSetDelete(sender, e);
        plotContextPlotDelete.Click += async (sender, e) => await controller.ContextSpectraDelete(sender, e);

        plotButtonAddPeak.Click += async (sender, e) => await controller.PlotAddPeakBorders(sender, e);
        plotButtonDeleteLastPeak.Click += async (sender, e) => await controller.PlotDeleteLastPeakBorders(sender, e);
        plotButtonClearPeaks.Click += async (sender, e) => await controller.PlotClearPeakBorders(sender, e);

        plotContextPlotSetPeaksProcess.Click += async (sender, e) => await controller.ContextSetPeaksProcess(sender, e);
        plotContextPlotPeaksProcess.Click += async (sender, e) => await controller.ContextDataPeaksProcess(sender, e);
    }

    private void DrawMouseCoordinates()
    {
        var point = controller.PlotCoordinates;
        mouseCoordinatesBox.Text = $@"X:{point.X: 0.00} {point.Y: 0.00}";
    }

    private void TreeNodeClickSelect(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (sender is TreeView treeView) treeView.SelectedNode = e.Node;
        else throw new Exception(nameof(TreeNodeClickSelect));
    }

    private void PlotSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button is MouseButtons.Right && e.Node.Tag is DataSet<Spectra>)
        {
            e.Node.ContextMenuStrip = plotSetMenu;
        }
    }

    private void PlotDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button is MouseButtons.Right && e.Node.Tag is Spectra)
            e.Node.ContextMenuStrip = plotMenu;
    }

    private void DataSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button is MouseButtons.Right && e.Node.Tag is DataSet<Spectra>)
            e.Node.ContextMenuStrip = dataSetMenu;
    }

    private void DataDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Button is MouseButtons.Right && e.Node.Tag is Spectra)
            e.Node.ContextMenuStrip = dataMenu;
    }

    private static async Task BuildTreeAsync(TreeView tree, Func<IEnumerable<TreeNode>> nodeSource)
    {
        tree.Nodes.Clear();
        tree.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        tree.Nodes.AddRange(nodes);
        tree.EndUpdate();
    }
}