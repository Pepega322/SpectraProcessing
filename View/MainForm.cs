using Model.DataFormats.Base;
using Model.Controllers.Windows;
using Model.DataStorages.Base;

namespace Model;

public partial class MainForm : Form
{
    private WinFormsController _controller;

    public MainForm()
    {
        InitializeComponent();
        _controller = new(plotView);
        _controller.OnRootChanged += () => BuildTreeViewAsync(treeViewRoot, _controller.GetRootTree);
        _controller.OnDataChanged += () => BuildTreeViewAsync(treeViewData, _controller.GetDataTree);
        _controller.OnPlotChanged += () => BuildTreeViewAsync(treeViewPlot, _controller.GetPlotTree);

        BuildTreeViewAsync(treeViewRoot, _controller.GetRootTree);
        BuildTreeViewAsync(treeViewData, _controller.GetDataTree);

        plotView.Plot.Palette = ScottPlot.Palette.Category20;
        plotView.Plot.XLabel("Raman shift, cm-1");
        plotView.Plot.YLabel("Intensity");

        buttonRootSelect.Click += (sender, args) => _controller.RootSelect();
        buttonRootReadAll.Click += (sender, args) => _controller.RootReadAllAsync();
        buttonRootReadThis.Click += (sender, args) => _controller.RootReadThisAsync();
        buttonRootBack.Click += (sender, args) => _controller.RootStepBack();
        treeViewRoot.NodeMouseDoubleClick += (sender, args) => _controller.RootDoubleClick(args);

        buttonDataUpdate.Click += (sender, args) => BuildTreeViewAsync(treeViewData, _controller.GetDataTree);
        treeViewData.NodeMouseClick += (sender, args) => DrawContextMenu(args);


        buttonContextNodeSaveThis.Click += (sender, args) => _controller.SaveThisSetAsESPAsync();
        buttonContextNodeSaveAll.Click += (sender, args) => _controller.SaveAllSetAsESPAsync();
        buttonContextNodeDelete.Click += (sender, args) => _controller.DeleteSet();
        buttonContextNodePlot.Click += (sender, args) => _controller.PlotContext();
        buttonContextNodeAddToPlot.Click += (sender, args) => _controller.AddContestToPlot();

        buttonContextDataSave.Click += (sender, args) => _controller.SaveAsESPAsync();
        buttonContextDataDelete.Click += (sender, args) => _controller.DeleteData();
        buttonContextDataPlot.Click += (sender, args) => _controller.PlotData();
        treeViewData.NodeMouseDoubleClick += (sender, args) => _controller.AddDataToPlotDoubleClick(args);

        plotView.MouseMove += (sender, args) => DrawMouseCoordinates();
        buttonPlotDataClear.Click += (sender, args) => _controller.ClearPlot();

        treeViewPlot.AfterCheck += (sender, args) => _controller.ChangePlotVisibility(args);
        //treeViewPlot.NodeMouseClick += (sender, args) => _controller.HighlightPlot(args);
    }

    private void DrawMouseCoordinates()
    {
        var (x, y) = plotView.GetMouseCoordinates();
        mouseCoordinatesBox.Text = $"X: {Math.Round(x, 1)}\tY: {Math.Round(y, 1)}";
    }

    private void DrawContextMenu(TreeNodeMouseClickEventArgs args)
    {
        if (args.Button is MouseButtons.Right)
        {
            if (args.Node.Tag is DataSetNode node)
            {
                args.Node.ContextMenuStrip = dataNodeContextMenu;
                _controller.ContextSet = node;
            }

            if (args.Node.Tag is Data data)
            {
                args.Node.ContextMenuStrip = dataContextMenu;
                _controller.ContextData = data;
                if (args.Node.Parent.Tag is not DataSetNode parentNode)
                    throw new Exception("DrawContextMenu Method works wrong");
                _controller.ContextSet = parentNode;
            }
        }
    }

    private static async void BuildTreeViewAsync(TreeView view, Func<IEnumerable<TreeNode>> nodeSource)
    {
        view.Nodes.Clear();
        view.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        view.Nodes.AddRange(nodes);
        view.EndUpdate();
    }
}
