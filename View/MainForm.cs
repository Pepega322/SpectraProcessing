using Model.DataFormats;
using Model.DataStorages;
using View.Controllers;

namespace View;

public partial class MainForm : Form {
    private WinFormsController controller;

    public MainForm() {
        InitializeComponent();
        controller = new(plotView);
        controller.OnRootChanged += async () => await BuildTreeViewAsync(treeViewRoot, controller.GetRootTree);
        controller.OnDataChanged += async () => await BuildTreeViewAsync(treeViewData, controller.GetDataTree);
        controller.OnPlotChanged += async () => await BuildTreeViewAsync(treeViewPlot, controller.GetPlotTree);

        _ = BuildTreeViewAsync(treeViewRoot, controller.GetRootTree);
        _ = BuildTreeViewAsync(treeViewData, controller.GetDataTree);

        buttonRootSelect.Click += (sender, args) => controller.RootSelect();
        buttonRootReadAll.Click += async (sender, args) => await controller.RootReadAllAsync();
        buttonRootReadThis.Click += async (sender, args) => await controller.RootReadThisAsync();
        buttonRootBack.Click += (sender, args) => controller.RootStepBack();
        treeViewRoot.NodeMouseDoubleClick += (sender, args) => controller.RootDoubleClick(args.Node.Tag);

        treeViewData.NodeMouseClick += (sender, args) => DrawContextMenu(args);
        buttonContextNodeSaveThis.Click += async (sender, args) => await controller.SaveSetAsESPAsync();
        buttonContextNodeSaveAll.Click += async (sender, args) => await controller.SaveSetAndSubsetsAsESPAsync();
        buttonContextNodeDelete.Click += (sender, args) => controller.DeleteSet();
        buttonContextNodePlot.Click += async (sender, args) => await controller.PlotContextAsync();
        //buttonContextNodeSubstractBaseline += 

        buttonContextNodeAddToPlot.Click += async (sender, args) => await controller.AddContestToPlotAsync();

        buttonContextDataSave.Click += async (sender, args) => await controller.SaveDataAsESPAsync();
        buttonContextDataDelete.Click += (sender, args) => controller.DeleteData();
        buttonContextDataPlot.Click += async (sender, args) => await controller.PlotDataAsync();
        treeViewData.NodeMouseDoubleClick += async (sender, args) => await controller.AddDataToPlotAsync(args.Node.Tag);
        //buttonContextDataSubstractBaseline += 

        //plotView.MouseMove += (sender, args) => DrawMouseCoordinates();
        buttonPlotDataClear.Click += (sender, args) => controller.ClearPlot();

        treeViewPlot.AfterCheck += (sender, args) => {
            if (args.Node is not null)
                controller.ChangePlotVisibility(args.Node.Tag, args.Node.Checked);
        };
        treeViewPlot.NodeMouseDoubleClick += (sender, args) => controller.SelectPlot(args.Node.Tag);
    }

    //private void DrawMouseCoordinates()
    //{
    //    var (x, y) = plotView.Plot.Coo();
    //    mouseCoordinatesBox.Text = $"X: {Math.Round(x, 1)}\tY: {Math.Round(y, 1)}";
    //}

    private void DrawContextMenu(TreeNodeMouseClickEventArgs args) {
        if (args.Button is MouseButtons.Right) {
            if (args.Node.Tag is DataSetNode set) {
                args.Node.ContextMenuStrip = dataSetContextMenu;
                controller.ChangeContextSet(set);
            }

            if (args.Node.Tag is Data data) {
                args.Node.ContextMenuStrip = dataContextMenu;
                controller.ChangeContextData(data);
                if (args.Node.Parent.Tag is not DataSetNode parentNode)
                    throw new Exception("DrawContextMenu Method works wrong");
                controller.ChangeContextSet(parentNode);
            }
        }
    }

    private async Task BuildTreeViewAsync(TreeView view, Func<IEnumerable<TreeNode>> nodeSource) {
        view.Nodes.Clear();
        view.BeginUpdate();
        var nodes = await Task.Run(() => nodeSource().ToArray());
        view.Nodes.AddRange(nodes);
        view.EndUpdate();
    }
}
