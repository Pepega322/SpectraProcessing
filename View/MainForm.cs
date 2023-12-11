using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.SupportedSpectraFormats.Base;
using View.Controllers.Windows;
using View.Storage;

namespace View;

public partial class MainForm : Form
{
    private readonly WindowsController _controller;

    public MainForm()
    {
        InitializeComponent();
        _controller = new("Storage");
        _controller.OnRootChange += () => UpdateTreeViewAsync(rootTreeView, _controller.GetRootNodes);
        _controller.OnDataChange += () => UpdateTreeViewAsync(dataTreeView, _controller.GetDataNodes);

        plotView.Plot.Palette = ScottPlot.Palette.Category20;
        plotView.Plot.XLabel("Raman shift, cm-1");
        plotView.Plot.YLabel("Intensity");

        rootSelectButton.Click += (sender, args) => _controller.RootSelect();
        rootReadAllButton.Click += (sender, args) => _controller.ReadAllRootAsync();
        rootReadThisButton.Click += (sender, args) => _controller.ReadThisRootAsync();

        rootBackButton.Click += (sender, args) => _controller.RootStepBack();
        rootTreeView.NodeMouseDoubleClick += (sender, args) => _controller.RootStepInDoubleClick(args);

        dataUpdateButton.Click += (sender, args) => UpdateTreeViewAsync(dataTreeView, _controller.GetDataNodes);
        dataTreeView.NodeMouseDoubleClick += (sender, args) => DataDoubleClick(args);

        dataTreeView.NodeMouseClick += (sender, args) => DrawContextMenu(args);

        dataNodeContextSaveThisButton.Click += (sender, args) => _controller.SaveThisSeriesAsESPAsync();
        dataNodeContextSaveAllButton.Click += (sender, args) => _controller.SaveAllSeriesAsESPAsync();
        //dataNodeContextPlotThisButton += (sender, args) => _controller.PlotThisSeries();
        //dataNodeContextPlotAllButton += (sender, args) => _controller.PlotAllSeries();
        //dataNodeContextAddToPlotThisButton += (sender, args) => _controller.AddToPlotThisSeries();
        //dataNodeContextAddToPlotAllButton += (sender, args) => _controller.AddToPlotAllSeries();
        dataNodeContextDeleteButton.Click += (sender, args) => _controller.DeleteNode();

        dataContextSaveAsESPButton.Click += (sender, args) => _controller.SaveAsESPAsync();
        dataContextAddToPlotButton.Click += (sender, args) => AddDataToPlot();
        dataContextDeleteButton.Click += (sender, args) => _controller.DeleteData();

        plotClearButton.Click += (sender, args) => ClearPlot();
        plotView.MouseMove += (sender, args) => DrawMouseCoordinates();

    }

    private void AddDataToPlot()
    {
        if (_controller.SelectedData is Spectra spectra)
        {
            var (sX, sY) = spectra.GetPoints();
            plotView.Plot.AddScatterLines(sX, sY);
            plotView.Refresh();
        }
    }

    private async void DataDoubleClick(TreeNodeMouseClickEventArgs args)
    {
        if (args.Node.Tag is Spectra spectra)
        {
            var (sX, sY) = await Task.Run(spectra.GetPoints);
            plotView.Plot.Clear();
            plotView.Plot.AddScatterLines(sX, sY);
            plotView.Plot.Title(spectra.Name);
            plotView.Refresh();
        }
    }

    private void DrawMouseCoordinates()
    {
        var (x, y) = plotView.GetMouseCoordinates();
        mouseCoordinatesBox.Text = $"X: {Math.Round(x, 1)}\tY: {Math.Round(y, 1)}";
    }

    private void DrawContextMenu(TreeNodeMouseClickEventArgs args)
    {
        if (args.Button != MouseButtons.Right) return;
        if (args.Node.Tag is DataNode node)
        {
            args.Node.ContextMenuStrip = dataNodeContextMenu;
            _controller.SelectedNode = node;
            return;
        }
        if (args.Node.Tag is Data data)
        {
            args.Node.ContextMenuStrip = dataContextMenu;
            _controller.SelectedData = data;
            if (args.Node.Parent.Tag is not DataNode parentNode)
                throw new Exception("args.Node.Parent.Tag is not DataNode, fix it");
            _controller.SelectedNode = parentNode;
        }
    }

    private static async void UpdateTreeViewAsync(TreeView view, Func<IEnumerable<TreeNode>> nodeSource)
    {
        view.Nodes.Clear();
        view.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        view.Nodes.AddRange(nodes);
        view.EndUpdate();
    }


    private void ClearPlot()
    {
        plotView.Plot.Clear();
        //plotView.Plot.Title("");
        //plotView.Plot.XLabel("");
        //plotView.Plot.YLabel("");
        plotView.Refresh();
    }
}
