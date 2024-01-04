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
        _controller.OnRootChanged += async () => await BuildTreeViewAsync(treeViewRoot, _controller.GetRootTree);
        _controller.OnDataChanged += async () => await BuildTreeViewAsync(treeViewData, _controller.GetDataTree);
        _controller.OnPlotChanged += async () => await BuildTreeViewAsync(treeViewPlot, _controller.GetPlotTree);

        _ = BuildTreeViewAsync(treeViewRoot, _controller.GetRootTree);
        _ = BuildTreeViewAsync(treeViewData, _controller.GetDataTree);

        buttonRootSelect.Click += (sender, args) => _controller.RootSelect();
        buttonRootReadAll.Click += async (sender, args) => await _controller.RootReadAllAsync();
        buttonRootReadThis.Click += async (sender, args) => await _controller.RootReadThisAsync();
        buttonRootBack.Click += (sender, args) => _controller.RootStepBack();
        treeViewRoot.NodeMouseDoubleClick += (sender, args) => _controller.RootDoubleClick(args.Node.Tag);

        treeViewData.NodeMouseClick += (sender, args) => DrawContextMenu(args);
        buttonContextNodeSaveThis.Click += async (sender, args) => await _controller.SaveThisSetAsESPAsync();
        buttonContextNodeSaveAll.Click += async (sender, args) => await _controller.SaveAllSetAsESPAsync();
        buttonContextNodeDelete.Click += (sender, args) => _controller.DeleteSet();
        buttonContextNodePlot.Click += async (sender, args) => await _controller.PlotContextAsync();

        buttonContextNodeAddToPlot.Click += async (sender, args) => await _controller.AddContestToPlotAsync();

        buttonContextDataSave.Click += async (sender, args) => await _controller.SaveAsESP();
        buttonContextDataDelete.Click += (sender, args) => _controller.DeleteData();
        buttonContextDataPlot.Click += async (sender, args) => await _controller.PlotDataAsync();
        treeViewData.NodeMouseDoubleClick += async (sender, args) => await _controller.AddDataToPlotAsync(args.Node.Tag);

        plotView.MouseMove += (sender, args) => DrawMouseCoordinates();
        buttonPlotDataClear.Click += (sender, args) => _controller.ClearPlot();

        treeViewPlot.AfterCheck += (sender, args) =>
        {
            if (args.Node is not null)
                _controller.ChangePlotVisibility(args.Node.Tag, args.Node.Checked);
        };
        treeViewPlot.NodeMouseDoubleClick += (sender, args) => _controller.SelectPlot(args.Node.Tag);
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

    private static async Task BuildTreeViewAsync(TreeView view, Func<IEnumerable<TreeNode>> nodeSource)
    {
        view.Nodes.Clear();
        view.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        view.Nodes.AddRange(nodes);
        view.EndUpdate();
    }
}
