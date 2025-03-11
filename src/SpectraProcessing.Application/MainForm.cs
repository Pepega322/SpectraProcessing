using Microsoft.Extensions.DependencyInjection;
using SpectraProcessing.Application.Controllers;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Domain;
using SpectraProcessing.Domain.SpectraData;
using SpectraProcessing.Domain.Storage;
using SpectraProcessing.Graphics.Formats;

namespace SpectraProcessing.Application;

public partial class MainForm : Form
{
    // readonly double[] Xs = Generate.RandomAscending(10);
    // readonly double[] Ys = Generate.RandomSample(10);
    // readonly ScottPlot.Plottables.Scatter Scatter;
    // int? IndexBeingDragged = null;
    //
    // public MainForm()
    // {
    //     InitializeComponent();
    //
    //     Scatter = plotView.Plot.Add.Scatter(Xs, Ys);
    //     Scatter.LineWidth = 2;
    //     Scatter.MarkerSize = 10;
    //     Scatter.Smooth = true;
    //
    //     plotView.MouseMove += FormsPlot1_MouseMove;
    //     plotView.MouseDown += FormsPlot1_MouseDown;
    //     plotView.MouseUp += FormsPlot1_MouseUp;
    // }
    //
    // private void FormsPlot1_MouseDown(object? sender, MouseEventArgs e)
    // {
    //     Pixel mousePixel = new(e.Location.X, e.Location.Y);
    //     Coordinates mouseLocation = plotView.Plot.GetCoordinates(mousePixel);
    //     DataPoint nearest = Scatter.Data.GetNearest(mouseLocation, plotView.Plot.LastRender);
    //     IndexBeingDragged = nearest.IsReal ? nearest.Index : null;
    //
    //     if (IndexBeingDragged.HasValue)
    //         plotView.UserInputProcessor.Disable();
    // }
    //
    // private void FormsPlot1_MouseUp(object? sender, MouseEventArgs e)
    // {
    //     IndexBeingDragged = null;
    //     plotView.UserInputProcessor.Enable();
    //     plotView.Refresh();
    // }
    //
    // private void FormsPlot1_MouseMove(object? sender, MouseEventArgs e)
    // {
    //     Pixel mousePixel = new(e.Location.X, e.Location.Y);
    //     Coordinates mouseLocation = plotView.Plot.GetCoordinates(mousePixel);
    //     DataPoint nearest = Scatter.Data.GetNearest(mouseLocation, plotView.Plot.LastRender);
    //     plotView.Cursor = nearest.IsReal ? Cursors.Hand : Cursors.Arrow;
    //
    //     if (IndexBeingDragged.HasValue)
    //     {
    //         Xs[IndexBeingDragged.Value] = mouseLocation.X;
    //         Ys[IndexBeingDragged.Value] = mouseLocation.Y;
    //         plotView.Refresh();
    //     }
    // }

    private readonly IDialogController dialogController;
    private readonly ICoordinateController coordinateController;
    private readonly IDataSourceController<Spectra> dataSourceController;
    private readonly IDataWriterController dataWriterController;
    private readonly IDataStorageController<Spectra> dataStorageController;
    private readonly IPlotController plotController;

    public MainForm()
    {
        InitializeComponent();
        var provider = Startup.GetServiceProvider(plotView);


        plotController = provider.GetRequiredService<IPlotController>();
        dialogController = provider.GetRequiredService<IDialogController>();
        dataStorageController = provider.GetRequiredService<IDataStorageController<Spectra>>();
        dataSourceController = provider.GetRequiredService<IDataSourceController<Spectra>>();
        dataWriterController = provider.GetRequiredService<IDataWriterController>();
        coordinateController = provider.GetRequiredService<ICoordinateController>();

        SetupDataReaderController();
        SetupDataStorageController();
        SetupPlotController();
        SetupCoordinateControllerController();
        // SetupSpectraProcessingController();

        dataContextMenu.Tag = dataStorageTreeView;
        dataSetContextMenu.Tag = dataStorageTreeView;
        dataStorageTreeView.NodeMouseClick += TreeNodeClickSelect;
        dataStorageTreeView.NodeMouseClick += DataDrawContextMenu;
        dataStorageTreeView.NodeMouseClick += DataSetDrawContextMenu;

        plotContextMenu.Tag = plotStorageTreeView;
        plotSetContextMenu.Tag = plotStorageTreeView;
        plotStorageTreeView.NodeMouseClick += TreeNodeClickSelect;
        plotStorageTreeView.NodeMouseClick += PlotDrawContextMenu;
        plotStorageTreeView.NodeMouseClick += PlotSetDrawContextMenu;
        plotStorageTreeView.NodeMouseDoubleClick += TreeNodeClickSelect;
    }

    private void SetupDataReaderController()
    {
        readFolderToolStripMenuItem.Click += async (_, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = await dataSourceController.ReadFolderAsync(path);

            dataStorageController.AddDataSet(set);
        };

        readFolderRecursiveToolStripMenuItem.Click += async (_, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = await dataSourceController.ReadFolderFullDepthAsync(path);

            dataStorageController.AddDataSet(set);
        };
    }

    private void SetupDataStorageController()
    {
        dataStorageController.OnChange +=
            async () => await dataStorageTreeView.BuildTreeAsync(dataStorageController.GetDataNodes);

        dataContextMenuClear.Click += (_, _) => dataStorageController.Clear();

        dataContextMenuSaveAsEsp.Click += async (sender, _) =>
        {
            var data = TreeViewHelpers.GetContextData<Spectra>(sender);

            var fullname = dialogController.GetSaveFileFullName(data.Name, ".esp");

            if (fullname is null)
            {
                return;
            }

            await Task.Run(() => dataWriterController.DataWriteAs(data, fullname));
        };

        dataContextMenuDelete.Click += (sender, _) =>
        {
            var ownerSet = TreeViewHelpers.GetContextParentSet<Spectra>(sender);
            var spectra = TreeViewHelpers.GetContextData<Spectra>(sender);
            dataStorageController.DeleteData(ownerSet, spectra);
        };

        dataSetContextMenuClear.Click += (_, _) => dataStorageController.Clear();

        dataSetContextMenuSaveAsEspCurrent.Click += async (sender, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = TreeViewHelpers.GetContextSet<Spectra>(sender);

            var outputPath = Path.Combine(path, $"{set.Name} (converted)");

            await dataWriterController.SetOnlyWriteAs(set, outputPath, ".esp");
        };

        dataSetContextMenuSaveAsEspRecursive.Click += async (sender, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = TreeViewHelpers.GetContextSet<Spectra>(sender);

            var outputPath = Path.Combine(path, $"{set.Name} (converted full depth)");

            await dataWriterController.SetFullDepthWriteAs(set, outputPath, ".esp");
        };

        dataSetContextMenuDelete.Click += (sender, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<Spectra>(sender);
            dataStorageController.DeleteDataSet(set);
        };

        //Plotting
        dataContextMenuPlot.Click += async (sender, _) =>
        {
            var spectra = TreeViewHelpers.GetContextData<Spectra>(sender);
            await plotController.ContextDataAddToClearPlotToDefault(spectra);
        };

        dataSetContextMenuPlot.Click += async (s, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<Spectra>(s);
            await plotController.ContextDataAddToClearPlotArea(set);
        };

        dataSetContextMenuAddToPlot.Click += async (s, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<Spectra>(s);
            await plotController.ContextDataSetAddToPlotArea(set);
        };

        dataStorageTreeView.NodeMouseDoubleClick += async (_, e) =>
        {
            if (e is { Button: MouseButtons.Left, Node.Tag: Spectra spectra })
            {
                await plotController.DataAddToPlotAreaToDefault(spectra);
            }
        };
    }

    private void SetupPlotController()
    {
        resizeToolStripMenuItem.Click += (_, _) => plotController.PlotAreaResize();

        plotController.OnPlotStorageChanged +=
            async () => await plotStorageTreeView.BuildTreeAsync(plotController.GetPlotNodes);

        plotController.OnPlotAreaChanged += () => plotView.Refresh();

        plotContextMenuClear.Click += (_, _) => plotController.PlotAreaClear();

        plotContextMenuDelete.Click += async (sender, _) =>
        {
            var ownerSet = TreeViewHelpers.GetContextParentSet<SpectraPlot>(sender);
            var plot = TreeViewHelpers.GetContextData<SpectraPlot>(sender);
            await plotController.ContextPlotDelete(ownerSet, plot);
        };

        plotStorageTreeView.AfterCheck += (_, e) =>
        {
            if (e is { Node.Tag: SpectraPlot plot })
            {
                plotController.ChangePlotVisibility(plot, e.Node.Checked).Wait();
            }
        };

        plotSetContextMenuDelete.Click += async (sender, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<SpectraPlot>(sender);
            await plotController.ContextPlotSetDelete(set);
        };

        plotStorageTreeView.NodeMouseDoubleClick += async (sender, _) =>
        {
            var node = TreeViewHelpers.GetClickTreeNode(sender);

            if (node is { Tag: SpectraPlot plot, Checked: true })
            {
                await plotController.PlotHighlight(plot);
            }
        };

        plotSetContextMenuClear.Click += (_, _) => plotController.PlotAreaClear();

        plotSetContextMenuHighlight.Click += async (sender, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<SpectraPlot>(sender);

            await plotController.ContextPlotSetHighlight(set);
        };

        plotStorageTreeView.AfterCheck += (_, e) =>
        {
            if (e is { Node.Tag: DataSet<SpectraPlot> set })
            {
                plotController.ChangePlotSetVisibility(set, e.Node.Checked).Wait();
            }
        };
    }

    private void SetupCoordinateControllerController()
    {
        coordinateController.OnChange += () =>
        {
            var c = coordinateController.Coordinates;
            mouseCoordinatesBox.Text = $@"X:{c.X: 0.00} Y:{c.Y: 0.00}";
        };

        plotView.MouseMove += (_, e) => coordinateController.Coordinates = new Point<float>(e.X, e.Y);
    }

    // private void SetupSpectraProcessingController()
    // {
    //     plotButtonImportPeaks.Click += async (_, _) =>
    //     {
    //         var fullName = dialogController.GetReadFileFullName();
    //         if (fullName is null)
    //         {
    //             return;
    //         }
    //
    //         await processingController.ImportBorders(fullName);
    //         plotView.Refresh();
    //     };
    //     plotButtonExportPeaks.Click += async (_, _) =>
    //     {
    //         var borderSet = new PeakBordersSet
    //         {
    //             Name = "",
    //             Borders = processingController.Borders.ToArray(),
    //         };
    //         var fullName = dialogController.GetSaveFileFullName("bordersSet", "borders");
    //         if (fullName is null)
    //         {
    //             return;
    //         }
    //
    //         await dataWriterController.DataWriteAs(borderSet, fullName);
    //     };
    //     plotButtonClearPeaks.Click += async (_, _) =>
    //     {
    //         await Task.Run(processingController.ClearBorders);
    //         plotView.Refresh();
    //     };
    //     plotContextPlotSetSubstractBaseLine.Click += async (sender, _) =>
    //     {
    //         var set = TreeViewHelpers.GetContextSet<SpectraPlot>(sender);
    //         var spectra = set.Data.Select(s => s.Spectra).ToArray();
    //         var substracted = await processingController.SubstractBaseline(spectra);
    //         var substractedSet = new DataSet<Spectra>($"{set.Name} b-", substracted);
    //         dataStorageController.AddDataSet(substractedSet);
    //     };
    //     plotContextPlotSubstractBaseLine.Click += async (sender, _) =>
    //     {
    //         var plot = TreeViewHelpers.GetContextData<SpectraPlot>(sender);
    //         var substracted = await processingController.SubstractBaseline(plot.Spectra);
    //         dataStorageController.AddDataToDefaultSet(substracted);
    //     };
    //     plotButtonAddPeak.Click += async (_, _) =>
    //     {
    //         var start = await coordinateController.GetCoordinateByClick();
    //         var end = await coordinateController.GetCoordinateByClick();
    //         var border = new PeakBorders(start.X, end.X);
    //         processingController.AddBorder(border);
    //         plotView.Refresh();
    //     };
    //     plotButtonDeleteLastPeak.Click += async (_, _) =>
    //     {
    //         var last = processingController.Borders.LastOrDefault();
    //         if (last is not null)
    //             await Task.Run(() => processingController.RemoveBorder(last));
    //         plotView.Refresh();
    //     };
    //     plotContextPlotSetPeaksProcess.Click += async (sender, _) =>
    //     {
    //         var plotSet = TreeViewHelpers.GetContextSet<SpectraPlot>(sender);
    //         var spectraSet = new DataSet<Spectra>(plotSet.Name, plotSet.Data.Select(plot => plot.Spectra));
    //         var processed = await processingController.ProcessPeaksForSpectraSet(spectraSet);
    //         var peaksFullname = dialogController.GetSaveFileFullName(plotSet.Name, processed.Extension);
    //         if (peaksFullname is null)
    //         {
    //             return;
    //         }
    //
    //         await dataWriterController.DataWriteAs(processed, peaksFullname);
    //         var dispersionStatistics = processed.GetDispersionStatistics();
    //         await dataWriterController.DataWriteAs(
    //             dispersionStatistics,
    //             $"{peaksFullname}.{dispersionStatistics.Extension}");
    //     };
    //     plotContextPlotPeaksProcess.Click += async (sender, _) =>
    //     {
    //         var plot = TreeViewHelpers.GetContextData<SpectraPlot>(sender);
    //         var fullname = dialogController.GetSaveFileFullName(plot.Name, ".txt");
    //         if (fullname is null)
    //         {
    //             return;
    //         }
    //
    //         var processed = await processingController.ProcessPeaksForSingleSpectra(plot.Spectra);
    //         await dataWriterController.DataWriteAs(processed, fullname);
    //     };
    //     plotContextPlotSetAverageSpectra.Click += async (s, _) =>
    //     {
    //         var set = TreeViewHelpers.GetContextSet<SpectraPlot>(s);
    //         var spectras = set.Data.Select(plot => plot.Spectra).ToArray();
    //         var average = await processingController.GetAverageSpectra(spectras);
    //         average.Name = $"{set.Name} (average)";
    //         dataStorageController.AddDataToDefaultSet(average);
    //     };
    // }

#region SupportMethods

    private static void TreeNodeClickSelect(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (sender is TreeView treeView)
        {
            treeView.SelectedNode = e.Node;
        }
    }

    private void PlotSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e is { Button: MouseButtons.Right, Node.Tag: DataSet<SpectraPlot> })
        {
            e.Node.ContextMenuStrip = plotSetContextMenu;
        }
    }

    private void PlotDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e is { Button: MouseButtons.Right, Node.Tag: SpectraPlot })
        {
            e.Node.ContextMenuStrip = plotContextMenu;
        }
    }

    private void DataSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e is { Button: MouseButtons.Right, Node.Tag: DataSet<Spectra> })
        {
            e.Node.ContextMenuStrip = dataSetContextMenu;
        }
    }

    private void DataDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e is { Button: MouseButtons.Right, Node.Tag: Spectra })
        {
            e.Node.ContextMenuStrip = dataContextMenu;
        }
    }

#endregion
}
