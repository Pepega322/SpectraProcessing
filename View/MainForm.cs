using Model.DataFormats;
using Model.DataStorages;
using View.Controllers;

namespace View;

public partial class MainForm : Form {
    private WinFormsController controller;

    public MainForm() {
        InitializeComponent();
        controller = new(plotView);
        controller.OnRootChanged += async () => await BuildTreeAsync(rootTree, controller.RootGetTree);
        controller.OnDataChanged += async () => await BuildTreeAsync(dataTree, controller.DataGetTree);
        controller.OnPlotChanged += async () => await BuildTreeAsync(plotTree, controller.PlotGetTree);

        _ = BuildTreeAsync(rootTree, controller.RootGetTree);
        _ = BuildTreeAsync(dataTree, controller.DataGetTree);

        rootButtonSelect.Click += (sender, args) => controller.RootSelect();
        rootButtonReadWithSubdirs.Click += async (sender, args) => await controller.RootReadWithSubdirsAsync();
        rootButtonRead.Click += async (sender, args) => await controller.RootReadAsync();
        rootButtonStepBack.Click += (sender, args) => controller.RootStepBack();
        rootButtonRefresh.Click += (sender, args) => controller.RootRefresh();
        rootTree.NodeMouseDoubleClick += (sender, args) => controller.RootDoubleClick(args.Node.Tag);

        dataTree.NodeMouseClick += (sender, args) => DataDrawContextMenu(args);
        dataButtonClear.Click += (sender, args) => controller.DataClear();

        dataButtonContextSetSaveAs.Click += async (sender, args) => await controller.ContextSetSaveAsESPAsync();
        dataButtonContextSetAndSubsetsSaveAs.Click += async (sender, args) => await controller.ContextSetAndSubsetsSaveAsESPAsync();
        dataButtonContextSetDelete.Click += (sender, args) => controller.ContextSetDelete();
        dataButtonContextSetPlot.Click += async (sender, args) => await controller.ContextSetPlotAsync();
        dataButtonContextSetAddToPlot.Click += async (sender, args) => await controller.ContextSetAddToPlotAsync();
        //dataButtonContextSetSubstractBaseline += 


        dataButtonContextDataSave.Click += async (sender, args) => await controller.ContextDataSaveAsESPAsync();
        dataButtonContextDataDelete.Click += (sender, args) => controller.ContextDataDelete();
        dataButtonContextDataPlot.Click += async (sender, args) => await controller.ContextDataPlotAsync();
        dataTree.NodeMouseDoubleClick += async (sender, args) => await controller.ContextDataAddToPlotAsync(args.Node.Tag);
        //dataButtonContextDataSubstractBaseline += 

        //plotView.MouseMove += (sender, args) => DrawMouseCoordinates();
        plotButtonClear.Click += (sender, args) => controller.PlotClear();

        plotTree.AfterCheck += (sender, args) => {
            if (args.Node is not null)
                controller.PlotChangeVisibility(args.Node.Tag, args.Node.Checked);
        };
        plotTree.NodeMouseDoubleClick += (sender, args) => controller.PlotSelect(args.Node.Tag);
    }

    //private void DrawMouseCoordinates()
    //{
    //    var (x, y) = plotView.Plot.Coo();
    //    mouseCoordinatesBox.Text = $"X: {Math.Round(x, 1)}\tY: {Math.Round(y, 1)}";
    //}

    private void DataDrawContextMenu(TreeNodeMouseClickEventArgs args) {
        if (args.Button is not MouseButtons.Right) return;
        if (args.Node.Tag is TreeSet set) {
            controller.ContextSetChange(set);
            args.Node.ContextMenuStrip = dataSetMenu;
            return;
        }

        if (args.Node.Tag is Data data) {
            controller.ContextDataChange(data);
            if (args.Node.Parent.Tag is not TreeSet parentNode)
                throw new Exception("DataDrawContextMenu Method works wrong");
            controller.ContextSetChange(parentNode);
            args.Node.ContextMenuStrip = dataMenu;
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
