using Model.SupportedDataFormats.SupportedSpectraFormats.Base;
using View.Controllers.Windows;

namespace View;

public partial class MainForm : Form
{
    private WindowsController _controller;

    public MainForm()
    {
        InitializeComponent();
        _controller = new("Storage");
        _controller.OnRootChange += () => UpdateTreeViewAsync(rootTreeView, _controller.GetRootNodes);
        _controller.OnDataAdd += () => UpdateTreeViewAsync(dataTreeView, _controller.GetDataNodes);

        rootSelectButton.Click += (sender, args) => _controller.RootSelect();
        rootTreeView.NodeMouseDoubleClick += (sender, args) => _controller.RootSelectDoubleClick(args);
        rootBackButton.Click += (sender, args) => _controller.RootStepBack();

        readRootAsOneButton.Click += (sender, args) => _controller.ReadRootAsSeriesAsync();
        dataTreeView.NodeMouseDoubleClick += (sender, args) => DrawPlotAsync(args);
    }

    private async void UpdateTreeViewAsync(TreeView view, Func<IEnumerable<TreeNode>> nodeSource)
    {
        view.Nodes.Clear();
        view.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        view.Nodes.AddRange(nodes);
        view.EndUpdate();
    }

    private async void DrawPlotAsync(TreeNodeMouseClickEventArgs args)
    {
        if (args.Node.Tag is Spectra spectra)
        {
            var points = await Task.Run(() => _controller.GetSpectraPoints(spectra));
            plotView.Plot.Clear();
            plotView.Plot.AddScatterLines(points.sX, points.sY, Color.Black);
            plotView.Plot.XLabel("Raman shift, cm-1");
            plotView.Plot.YLabel("Intensity");
            plotView.Plot.Title(spectra.Name);
            plotView.Refresh();
        }
    }
}
