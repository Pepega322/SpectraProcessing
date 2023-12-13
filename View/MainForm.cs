using Model.DataFormats.Base;
using Model.DataFormats.Spectras.Base;
using Model.Controllers.Windows;
using Model.DataStorages.Base;

namespace Model;

public partial class MainForm : Form
{
    private readonly WindowsController _controller;
    private event Action OnPlotChange;

    public MainForm()
    {
        InitializeComponent();
        _controller = new("Storage");
        _controller.OnRootChange += () => UpdateTreeViewAsync(treeViewRoot, _controller.GetRootNodes);
        _controller.OnDataChange += () => UpdateTreeViewAsync(treeViewData, _controller.GetDataNodes);
        UpdateTreeViewAsync(treeViewRoot, _controller.GetRootNodes);

        OnPlotChange += plotView.Refresh;
        plotView.Plot.Palette = ScottPlot.Palette.Category20;
        plotView.Plot.XLabel("Raman shift, cm-1");
        plotView.Plot.YLabel("Intensity");

        buttonRootSelect.Click += (sender, args) => _controller.RootSelect();
        buttonRootReadAll.Click += (sender, args) => _controller.RootReadAllAsync();
        buttonRootReadThis.Click += (sender, args) => _controller.RootReadThisAsync();
        buttonRootBack.Click += (sender, args) => _controller.RootStepBack();
        treeViewRoot.NodeMouseDoubleClick += (sender, args) => _controller.RootStepInDoubleClick(args);

        buttonDataUpdate.Click += (sender, args) => UpdateTreeViewAsync(treeViewData, _controller.GetDataNodes);
        treeViewData.NodeMouseDoubleClick += (sender, args) => AddDataToPlotDoubleClickAsync(args);
        treeViewData.NodeMouseClick += (sender, args) => DrawContextMenu(args);


        buttonContextNodeSaveThis.Click += (sender, args) => _controller.SaveThisSeriesAsESPAsync();
        buttonContextNodeSaveAll.Click += (sender, args) => _controller.SaveAllSeriesAsESPAsync();
        buttonContextNodePlot.Click += (sender, args) => PlotNodeAsync();
        buttonContextNodeAddToPlot.Click += (sender, args) => AddNodeToPlotAsync();
        buttonContextNodeDelete.Click += (sender, args) => _controller.DeleteNode();

        buttonContextDataSave.Click += (sender, args) => _controller.SaveAsESPAsync();
        buttonContextDataPlot.Click += (sender, args) => PlotDataAsync();
        buttonContextDataDelete.Click += (sender, args) => _controller.DeleteData();

        plotClearButton.Click += (sender, args) => ClearPlot();
        plotView.MouseMove += (sender, args) => DrawMouseCoordinates();

    }

    private async void PlotNodeAsync()
    {
        plotView.Plot.Clear();
        await Task.Run(AddNodeToPlotAsync);
    }

    private async void PlotDataAsync()
    {
        if (_controller.SelectedData is Spectra spectra)
        {
            plotView.Plot.Clear();
            await Task.Run(() => AddSpectraToPlot(spectra));
            OnPlotChange?.Invoke();
        }
    }

    private async void AddNodeToPlotAsync()
    {
        await Task.Run(() => AddNodeSpectraToPlot(_controller.SelectedSet));
        OnPlotChange?.Invoke();
    }

    private async void AddDataToPlotDoubleClickAsync(TreeNodeMouseClickEventArgs args)
    {
        if (args.Node.Tag is Spectra spectra)
        {
            await Task.Run(() => AddSpectraToPlot(spectra));
            OnPlotChange?.Invoke();
        }
    }

    private void AddNodeSpectraToPlot(DataSetNode node)
    {
        foreach (var data in node.Data.Where(d => d is Spectra))
            Task.Run(() => AddSpectraToPlot((Spectra)data));
    }

    private void AddSpectraToPlot(Spectra spectra)
    {
        var (sX, sY) = spectra.GetPoints();
        lock (plotView.Plot) plotView.Plot.AddScatterLines(sX.ToArray(), sY);
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
                _controller.SelectedSet = node;
            }

            if (args.Node.Tag is Data data)
            {
                args.Node.ContextMenuStrip = dataContextMenu;
                _controller.SelectedData = data;
                if (args.Node.Parent.Tag is not DataSetNode parentNode)
                    throw new Exception("DrawContextMenu Method works wrong");
                _controller.SelectedSet = parentNode;
            }
        }
    }

    private void ClearPlot()
    {
        plotView.Plot.Clear();
        OnPlotChange?.Invoke();
    }

    private static async void UpdateTreeViewAsync(TreeView view, Func<IEnumerable<TreeNode>> nodeSource)
    {
        view.Nodes.Clear();
        view.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        view.Nodes.AddRange(nodes);
        view.EndUpdate();
    }
}
