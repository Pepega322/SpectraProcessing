using Microsoft.Extensions.DependencyInjection;
using SpectraProcessing.Application.Extensions;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Collections.Keys;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Application;

public sealed partial class MainForm : Form
{
    private readonly IDataProvider<SpectraData> spectraDataProvider;
    private readonly IDataProvider<PeakDataSet> peakDataProvider;
    private readonly ICoordinateProvider coordinateProvider;
    private readonly IDataStorageProvider<StringKey, SpectraData> dataStorageProvider;

    private readonly IDialogController dialogController;
    private readonly IPeakController peakController;
    private readonly ISpectraController spectraController;
    private readonly IProcessingController processingController;

    public MainForm()
    {
        WindowState = FormWindowState.Maximized;

        InitializeComponent();

        var provider = Startup.GetServiceProvider(plotView);

        spectraController = provider.GetRequiredService<ISpectraController>();
        dialogController = provider.GetRequiredService<IDialogController>();
        dataStorageProvider = provider.GetRequiredService<IDataStorageProvider<StringKey, SpectraData>>();
        spectraDataProvider = provider.GetRequiredService<IDataProvider<SpectraData>>();
        peakDataProvider = provider.GetRequiredService<IDataProvider<PeakDataSet>>();
        coordinateProvider = provider.GetRequiredService<ICoordinateProvider>();
        peakController = provider.GetRequiredService<IPeakController>();
        processingController = provider.GetRequiredService<IProcessingController>();

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
    }

    private async Task<int> Prepare()
    {
        var set = await spectraDataProvider.ReadFolderAsync("d:\\Study\\Chemfuck\\диплом\\DEV\\");

        await dataStorageProvider.AddDataSet(new StringKey(set.Name), set);

        var spectra = set.Data.Single(s => s.Name == "Gauss.esp");

        await spectraController.DataAddToPlotAreaToDefault(spectra);

        await spectraController.PlotAreaResize();

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

            var set = await spectraDataProvider.ReadFolderFullDepthAsync(path);

            await dataStorageProvider.AddDataSet(new StringKey(set.Name), set);
        };
    }

    private void SetupDataStorageController()
    {
        dataStorageProvider.OnChange +=
            async () => await dataStorageTreeView.BuildTreeAsync(
                dataStorageProvider.Sets.Values
                    .Concat([dataStorageProvider.DefaultSet])
                    .GetDataNodes);

        dataContextMenuClear.Click += (_, _) => dataStorageProvider.Clear();

        dataContextMenuSaveAsEsp.Click += async (sender, _) =>
        {
            var data = TreeViewExtensions.GetContextData<SpectraData>(sender);

            var fullname = dialogController.GetSaveFileFullName(data!.Name, ".esp");

            if (fullname is null)
            {
                return;
            }

            await spectraDataProvider.DataWriteAs(data, fullname);
        };

        dataContextMenuDelete.Click += async (sender, _) =>
        {
            var ownerSet = TreeViewExtensions.GetContextParentSet<SpectraData>(sender);
            var spectra = TreeViewExtensions.GetContextData<SpectraData>(sender);
            await dataStorageProvider.DeleteData(ownerSet!, spectra!);
        };

        dataSetContextMenuClear.Click += (_, _) => dataStorageProvider.Clear();

        dataSetContextMenuSaveAsEspCurrent.Click += async (sender, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = TreeViewExtensions.GetContextSet<SpectraData>(sender);

            var outputPath = Path.Combine(path, $"{set!.Name} (converted)");

            await spectraDataProvider.DataWriteAs(set.Data, outputPath, ".esp");
        };

        dataSetContextMenuSaveAsEspRecursive.Click += async (sender, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = TreeViewExtensions.GetContextSet<SpectraData>(sender);

            var outputPath = Path.Combine(path, $"{set!.Name} (converted full depth)");

            await spectraDataProvider.SetWriteAs(set, outputPath, ".esp");
        };

        dataSetContextMenuDelete.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraData>(sender);
            await dataStorageProvider.DeleteDataSet(set!);
        };

        //Plotting
        dataContextMenuPlot.Click += async (sender, _) =>
        {
            var spectra = TreeViewExtensions.GetContextData<SpectraData>(sender);
            await spectraController.ContextDataAddToClearPlotToDefault(spectra!);
        };

        dataSetContextMenuPlot.Click += async (s, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraData>(s);
            await spectraController.ContextDataAddToClearPlotArea(set!);
        };

        dataSetContextMenuAddToPlot.Click += async (s, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraData>(s);
            await spectraController.AddToPlotArea(set!);
        };

        dataStorageTreeView.NodeMouseDoubleClick += async (_, e) =>
        {
            if (e is { Button: MouseButtons.Left, Node.Tag: SpectraData spectra })
            {
                await spectraController.DataAddToPlotAreaToDefault(spectra);
            }
        };
    }

    private void SetupPlotController()
    {
        resizeToolStripMenuItem.Click += async (_, _) => await spectraController.PlotAreaResize();

        spectraController.OnPlotStorageChanged +=
            async () => await plotStorageTreeView.BuildTreeAsync(spectraController.GetPlotNodes);

        spectraController.OnPlotAreaChanged += () => plotView.Refresh();

        plotContextMenuClear.Click += async (_, _) =>
        {
            await spectraController.PlotAreaClear();
            await processingController.ClearPeaks();
        };

        plotContextMenuDelete.Click += async (sender, _) =>
        {
            var ownerSet = TreeViewExtensions.GetContextParentSet<SpectraDataPlot>(sender);

            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            await spectraController.ContextPlotDelete(ownerSet!, plot!);

            await processingController.RemovePeaks(plot!.SpectraData);

            await spectraController.PlotRemoveHighlight(plot);
        };

        plotStorageTreeView.AfterCheck += async (_, e) =>
        {
            switch (e.Node?.Tag)
            {
                case SpectraDataPlot plot:
                    await spectraController.ChangePlotVisibility(plot, e.Node.Checked);
                    break;
                case DataSet<SpectraDataPlot> set:
                    await spectraController.ChangePlotSetVisibility(set, e.Node.Checked);
                    break;
            }
        };

        plotSetContextMenuDelete.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);
            await spectraController.ContextPlotSetDelete(set!);

            foreach (var plot in set!.Data)
            {
                await processingController.RemovePeaks(plot.SpectraData);
                await spectraController.PlotRemoveHighlight(plot);
            }
        };

        plotSetContextMenuClear.Click += async (_, _) =>
        {
            await spectraController.PlotAreaClear();
            await processingController.ClearPeaks();
        };

        plotSetContextMenuHighlight.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            await spectraController.ContextPlotSetHighlight(set!);
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
        processingController.OnPlotAreaChanged += plotView.Refresh;
        peakController.OnPeakChanges += plotView.Refresh;

        plotView.MouseDown += async (_, _) => await peakController.TryMovePeak();

        customPeaksToolStripMenuItem.Click += async (_, _) =>
        {
            if (customPeaksToolStripMenuItem.Checked)
            {
                customPeaksToolStripMenuItem.Checked = await processingController.SaveCurrentSpectraPeaks();
            }
            else
            {
                customPeaksToolStripMenuItem.Checked = await processingController.RemoveCurrentSpectraPeaks() is false;
            }
        };

        clearPeaksToolStripMenuItem.Click += async (_, _) => await processingController.ClearCurrentSpectraPeaks();

        plotView.SKControl!.MouseDoubleClick += async (_, _) =>
        {
            if (addOrRemovePeaksToolStripMenuItem.Checked is false)
            {
                return;
            }

            var peak = await peakController.TryGetPeak();

            if (peak is not null)
            {
                await processingController.RemovePeaksForCurrentSpectra([peak.Peak]);
            }
            else
            {
                await processingController.AddPeaksForCurrentSpectra(
                [
                    new PeakData(
                        center: coordinateProvider.Coordinates.X,
                        amplitude: coordinateProvider.Coordinates.Y,
                        halfWidth: 30f,
                        gaussianContribution: 1f),
                ]);
            }
        };

        plotStorageTreeView.NodeMouseDoubleClick += async (sender, _) =>
        {
            var node = TreeViewExtensions.GetClickTreeNode(sender);

            await CheckoutPlotNode(node!);
        };

        plotContextMenuSmooth.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            await processingController.SmoothSpectras([plot!.SpectraData]);
        };

        plotSetContextMenuSmooth.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            var spectras = set!.Data.Select(d => d.SpectraData).ToArray();

            await processingController.SmoothSpectras(spectras);
        };

        plotContextMenuFitPeaks.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            await processingController.FitPeaks([plot!.SpectraData]);
        };

        plotSetContextMenuFitPeaks.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            var spectras = set!.Data.Select(d => d.SpectraData).ToArray();

            await processingController.FitPeaks(spectras);
        };

        exportPeaksToolStripMenuItem.Click += async (_, _) =>
        {
            var plot = TreeViewExtensions.GetClickData<SpectraDataPlot>(plotContextMenu.Tag);

            var name = plot?.Name ?? "peaksSet";

            var fullname = dialogController.GetSaveFileFullName(name, PeakDataSet.FileExtension);

            if (fullname is null)
            {
                return;
            }

            var peaks = processingController.CurrentPeaks
                .Select(p => p.Peak)
                .ToArray();

            var set = new PeakDataSet(peaks, name);

            await peakDataProvider.DataWriteAs(set, fullname);
        };

        plotContextMenuExportPeaks.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            var peaks = await processingController.ExportPeaks(plot!.SpectraData);

            if (peaks.IsEmpty())
            {
                return;
            }

            var fullname = dialogController.GetSaveFileFullName(plot.Name, PeakDataSet.FileExtension);

            if (fullname is null)
            {
                return;
            }

            var set = new PeakDataSet(peaks, plot.Name);

            await peakDataProvider.DataWriteAs(set, fullname);
        };

        plotSetContextMenuExportPeaks.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            if (set!.Data.IsEmpty())
            {
                return;
            }

            var peaksSetsSet = new List<PeakDataSet>();

            foreach (var plot in set.Data)
            {
                var peaks = await processingController.ExportPeaks(plot.SpectraData);

                if (peaks.IsEmpty())
                {
                    continue;
                }

                peaksSetsSet.Add(new PeakDataSet(peaks, plot.Name));
            }

            if (peaksSetsSet.IsEmpty())
            {
                return;
            }

            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var outputPath = Path.Combine(path, $"{set!.Name} (peaks)");

            await peakDataProvider.DataWriteAs(peaksSetsSet, outputPath, PeakDataSet.FileExtension);
        };

        importPeaksToolStripMenuItem.Click += async (_, _) =>
        {
            var fullName = dialogController.GetReadFileFullName();

            if (fullName is null)
            {
                return;
            }

            var set = await peakDataProvider.ReadDataAsync(fullName);

            await processingController.AddPeaksForCurrentSpectra(set.Peaks);
        };

        plotContextMenuImportPeaks.Click += async (sender, _) =>
        {
            var fullName = dialogController.GetReadFileFullName();

            if (fullName is null)
            {
                return;
            }

            var peaksSet = await peakDataProvider.ReadDataAsync(fullName);

            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender, out var node);

            await processingController.ImportPeaks(plot!.SpectraData, peaksSet.Peaks);

            await CheckoutPlotNode(node);
        };

        plotSetContextMenuImportPeaks.Click += async (sender, _) =>
        {
            var fullName = dialogController.GetReadFileFullName();

            if (fullName is null)
            {
                return;
            }

            var peaksSet = await peakDataProvider.ReadDataAsync(fullName);

            var plotSet = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            foreach (var spectraData in plotSet!.Data.Select(p => p.SpectraData))
            {
                await processingController.ImportPeaks(spectraData, peaksSet.Peaks);
            }
        };
    }

#region SupportMethods

    private async Task CheckoutPlotNode(TreeNode node)
    {
        if (node is not { Tag: SpectraDataPlot plot, Checked: true })
        {
            return;
        }

        var isHighlight = await spectraController.PlotHighlight(plot);

        if (!isHighlight)
        {
            customPeaksToolStripMenuItem.Checked = await processingController.CheckoutSpectra(null);
            return;
        }

        customPeaksToolStripMenuItem.Checked = await processingController.CheckoutSpectra(plot.SpectraData);

        await HighlightNodeUntilNextClick(node);
    }

    private async Task HighlightNodeUntilNextClick(TreeNode node)
    {
        node.NodeFont = new Font(plotStorageTreeView.Font, FontStyle.Bold);

        var tcs = new TaskCompletionSource();

        TreeNodeMouseClickEventHandler? moveBackFont = (_, _) =>
        {
            node.NodeFont = new Font(plotStorageTreeView.Font, FontStyle.Regular);
            tcs.TrySetResult();
        };

        plotStorageTreeView.NodeMouseDoubleClick += moveBackFont;


        await tcs.Task;

        plotStorageTreeView.NodeMouseDoubleClick -= moveBackFont;
    }

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
