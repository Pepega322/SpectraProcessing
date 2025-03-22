using Microsoft.Extensions.DependencyInjection;
using SpectraProcessing.Application.Controllers;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.MathStatistics;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Collections.Keys;
using SpectraProcessing.Models.Peak;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Application;

public partial class MainForm : Form
{
    private readonly IDialogController dialogController;
    private readonly IDataSourceController<SpectraData> dataSourceController;
    private readonly IDataWriterController dataWriterController;
    private readonly IDataStorageController<StringKey, SpectraData> dataStorageController;
    private readonly ISpectraProcessingController spectraProcessingController;
    private readonly IPlotController plotController;
    private readonly IPeakController peakController;
    private readonly ICoordinateProvider coordinateProvider;

    public MainForm()
    {
        InitializeComponent();

        var provider = Startup.GetServiceProvider(plotView);
        plotController = provider.GetRequiredService<IPlotController>();
        dialogController = provider.GetRequiredService<IDialogController>();
        dataStorageController = provider.GetRequiredService<IDataStorageController<StringKey, SpectraData>>();
        dataSourceController = provider.GetRequiredService<IDataSourceController<SpectraData>>();
        dataWriterController = provider.GetRequiredService<IDataWriterController>();
        coordinateProvider = provider.GetRequiredService<ICoordinateProvider>();
        peakController = provider.GetRequiredService<IPeakController>();
        spectraProcessingController = provider.GetRequiredService<ISpectraProcessingController>();

        SetupDataReaderController();
        SetupDataStorageController();
        SetupPlotController();
        SetupCoordinateProvider();
        SetupSpectraProcessingController();

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

        PeakData[] estimates =
        [
            new(200, 30, 50, 0),
            new(440, 15, 50, 0),
            new(500, 30, 50, 0),
            new(740, 10, 50, 0),
            new(800, 30, 50, 0),
            new(850, 20, 50, 0),
        ];

        foreach (var estimate in estimates)
        {
            var fl = 1f;

            var plot = plotView.Plot.Add.Function(x => MathFunctions.GaussianAndLorentzianMix(x, estimate));
        }

        plotView.Plot.Add.Function(x => MathFunctions.GaussianAndLorentzianMix(x, estimates));
    }

    private async Task<int> Prepare()
    {
        var set = await dataSourceController.ReadFolderAsync("d:\\Study\\Chemfuck\\диплом\\DEV\\");

        await dataStorageController.AddDataSet(new StringKey(set.Name), set);

        var spectra = set.Data.Single(s => s.Name == "Gauss.esp");

        await plotController.DataAddToPlotAreaToDefault(spectra);

        await plotController.PlotAreaResize();

        plotView.Refresh();

        return 1;
    }

    private void SetupDataReaderController()
    {
        readFolderToolStripMenuItem.Click += async (_, _) =>
        {
            await Prepare();
            // var path = dialogController.GetFolderPath();
            //
            // if (path is null)
            // {
            //     return;
            // }
            //
            // var set = await dataSourceController.ReadFolderAsync(path);
            //
            // await dataStorageController.AddDataSet(set);
        };

        readFolderRecursiveToolStripMenuItem.Click += async (_, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = await dataSourceController.ReadFolderFullDepthAsync(path);

            await dataStorageController.AddDataSet(new StringKey(set.Name), set);
        };
    }

    private void SetupDataStorageController()
    {
        dataStorageController.OnChange +=
            async () => await dataStorageTreeView.BuildTreeAsync(dataStorageController.GetDataNodes);

        dataContextMenuClear.Click += (_, _) => dataStorageController.Clear();

        dataContextMenuSaveAsEsp.Click += async (sender, _) =>
        {
            var data = TreeViewHelpers.GetContextData<SpectraData>(sender);

            var fullname = dialogController.GetSaveFileFullName(data.Name, ".esp");

            if (fullname is null)
            {
                return;
            }

            await dataWriterController.DataWriteAs(data, fullname);
        };

        dataContextMenuDelete.Click += async (sender, _) =>
        {
            var ownerSet = TreeViewHelpers.GetContextParentSet<SpectraData>(sender);
            var spectra = TreeViewHelpers.GetContextData<SpectraData>(sender);
            await dataStorageController.DeleteData(ownerSet, spectra);
        };

        dataSetContextMenuClear.Click += (_, _) => dataStorageController.Clear();

        dataSetContextMenuSaveAsEspCurrent.Click += async (sender, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = TreeViewHelpers.GetContextSet<SpectraData>(sender);

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

            var set = TreeViewHelpers.GetContextSet<SpectraData>(sender);

            var outputPath = Path.Combine(path, $"{set.Name} (converted full depth)");

            await dataWriterController.SetFullDepthWriteAs(set, outputPath, ".esp");
        };

        dataSetContextMenuDelete.Click += async (sender, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<SpectraData>(sender);
            await dataStorageController.DeleteDataSet(new StringKey(set.Name), set);
        };

        //Plotting
        dataContextMenuPlot.Click += async (sender, _) =>
        {
            var spectra = TreeViewHelpers.GetContextData<SpectraData>(sender);
            await plotController.ContextDataAddToClearPlotToDefault(spectra);
        };

        dataSetContextMenuPlot.Click += async (s, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<SpectraData>(s);
            await plotController.ContextDataAddToClearPlotArea(set);
        };

        dataSetContextMenuAddToPlot.Click += async (s, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<SpectraData>(s);
            await plotController.ContextDataSetAddToPlotArea(set);
        };

        dataStorageTreeView.NodeMouseDoubleClick += async (_, e) =>
        {
            if (e is { Button: MouseButtons.Left, Node.Tag: SpectraData spectra })
            {
                await plotController.DataAddToPlotAreaToDefault(spectra);
            }
        };
    }

    private void SetupPlotController()
    {
        resizeToolStripMenuItem.Click += async (_, _) => await plotController.PlotAreaResize();

        plotController.OnPlotStorageChanged +=
            async () => await plotStorageTreeView.BuildTreeAsync(plotController.GetPlotNodes);

        plotController.OnPlotAreaChanged += () => plotView.Refresh();

        plotContextMenuClear.Click += async (_, _) => await plotController.PlotAreaClear();

        plotContextMenuDelete.Click += async (sender, _) =>
        {
            var ownerSet = TreeViewHelpers.GetContextParentSet<SpectraDataPlot>(sender);
            var plot = TreeViewHelpers.GetContextData<SpectraDataPlot>(sender);
            await plotController.ContextPlotDelete(ownerSet, plot);
        };

        plotStorageTreeView.AfterCheck += async (_, e) =>
        {
            switch (e.Node?.Tag)
            {
                case SpectraDataPlot plot:
                    await plotController.ChangePlotVisibility(plot, e.Node.Checked);
                    break;
                case DataSet<SpectraDataPlot> set:
                    await plotController.ChangePlotSetVisibility(set, e.Node.Checked);
                    break;
            }
        };

        plotSetContextMenuDelete.Click += async (sender, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<SpectraDataPlot>(sender);
            await plotController.ContextPlotSetDelete(set);
        };

        plotStorageTreeView.NodeMouseDoubleClick += async (sender, _) =>
        {
            var node = TreeViewHelpers.GetClickTreeNode(sender);

            if (node is { Tag: SpectraDataPlot plot, Checked: true })
            {
                await plotController.PlotHighlight(plot);
                await spectraProcessingController.SaveSpectraPeaks(plot.SpectraData);
            }
        };

        plotSetContextMenuClear.Click += (_, _) => plotController.PlotAreaClear();

        plotSetContextMenuHighlight.Click += async (sender, _) =>
        {
            var set = TreeViewHelpers.GetContextSet<SpectraDataPlot>(sender);

            await plotController.ContextPlotSetHighlight(set);
        };
    }

    private void SetupCoordinateProvider()
    {
        plotView.MouseMove += (_, e) =>
        {
            coordinateProvider.UpdateCoordinates(e.X, e.Y);
            var c = coordinateProvider.Coordinates;
            mouseCoordinatesBox.Text = $@"X:{c.X: 0.00} Y:{c.Y: 0.00}";
        };
    }

    private void SetupSpectraProcessingController()
    {
        spectraProcessingController.OnPlotAreaChanged += plotView.Refresh;

        addPeaksToolStripMenuItem.Click += (_, _) =>
        {
            if (addPeaksToolStripMenuItem.Checked)
            {
                removePeaksToolStripMenuItem.Checked = false;
            }
        };

        removePeaksToolStripMenuItem.Click += (_, _) =>
        {
            if (removePeaksToolStripMenuItem.Checked)
            {
                addPeaksToolStripMenuItem.Checked = false;
            }
        };

        plotView.SKControl!.MouseDoubleClick += async (_, _) =>
        {
            if (addPeaksToolStripMenuItem.Checked is false)
            {
                return;
            }

            var estimate = new PeakData(
                center: coordinateProvider.Coordinates.X,
                amplitude: coordinateProvider.Coordinates.Y,
                halfWidth: 30f);

            await spectraProcessingController.AddPeak(estimate);
        };

        plotView.SKControl!.MouseDoubleClick += async (_, _) =>
        {
            if (!removePeaksToolStripMenuItem.Checked)
            {
                return;
            }

            var peak = await peakController.TryGetPeak();

            if (peak is not null)
            {
                await spectraProcessingController.RemovePeak(peak.Peak);
            }
        };

        plotView.MouseDown += async (_, _) => await peakController.TryMovePeak();
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
        if (e is { Button: MouseButtons.Right, Node.Tag: DataSet<SpectraDataPlot> })
        {
            e.Node.ContextMenuStrip = plotSetContextMenu;
        }
    }

    private void PlotDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e is { Button: MouseButtons.Right, Node.Tag: SpectraDataPlot })
        {
            e.Node.ContextMenuStrip = plotContextMenu;
        }
    }

    private void DataSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e is { Button: MouseButtons.Right, Node.Tag: DataSet<SpectraData> })
        {
            e.Node.ContextMenuStrip = dataSetContextMenu;
        }
    }

    private void DataDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e is { Button: MouseButtons.Right, Node.Tag: SpectraData })
        {
            e.Node.ContextMenuStrip = dataContextMenu;
        }
    }

#endregion
}
