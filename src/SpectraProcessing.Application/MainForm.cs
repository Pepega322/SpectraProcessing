using Microsoft.Extensions.DependencyInjection;
using SpectraProcessing.Application.Extensions;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Collections.Keys;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Application;

public sealed partial class MainForm : Form
{
    private readonly IDataProvider<SpectraData> dataProvider;
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
        dataProvider = provider.GetRequiredService<IDataProvider<SpectraData>>();
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
        var set = await dataProvider.ReadFolderAsync("d:\\Study\\Chemfuck\\диплом\\DEV\\");

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

            var set = await dataProvider.ReadFolderFullDepthAsync(path);

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

            var fullname = dialogController.GetSaveFileFullName(data.Name, ".esp");

            if (fullname is null)
            {
                return;
            }

            await dataProvider.DataWriteAs(data, fullname);
        };

        dataContextMenuDelete.Click += async (sender, _) =>
        {
            var ownerSet = TreeViewExtensions.GetContextParentSet<SpectraData>(sender);
            var spectra = TreeViewExtensions.GetContextData<SpectraData>(sender);
            await dataStorageProvider.DeleteData(ownerSet, spectra);
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

            var outputPath = Path.Combine(path, $"{set.Name} (converted)");

            await dataProvider.SetOnlyWriteAs(set, outputPath, ".esp");
        };

        dataSetContextMenuSaveAsEspRecursive.Click += async (sender, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = TreeViewExtensions.GetContextSet<SpectraData>(sender);

            var outputPath = Path.Combine(path, $"{set.Name} (converted full depth)");

            await dataProvider.SetFullDepthWriteAs(set, outputPath, ".esp");
        };

        dataSetContextMenuDelete.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraData>(sender);
            await dataStorageProvider.DeleteDataSet(set);
        };

        //Plotting
        dataContextMenuPlot.Click += async (sender, _) =>
        {
            var spectra = TreeViewExtensions.GetContextData<SpectraData>(sender);
            await spectraController.ContextDataAddToClearPlotToDefault(spectra);
        };

        dataSetContextMenuPlot.Click += async (s, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraData>(s);
            await spectraController.ContextDataAddToClearPlotArea(set);
        };

        dataSetContextMenuAddToPlot.Click += async (s, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraData>(s);
            await spectraController.AddToPlotArea(set);
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

        plotContextMenuClear.Click += async (_, _) => await spectraController.PlotAreaClear();

        plotContextMenuDelete.Click += async (sender, _) =>
        {
            var ownerSet = TreeViewExtensions.GetContextParentSet<SpectraDataPlot>(sender);
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);
            await spectraController.ContextPlotDelete(ownerSet, plot);
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
            await spectraController.ContextPlotSetDelete(set);
        };

        plotSetContextMenuClear.Click += (_, _) => spectraController.PlotAreaClear();

        plotSetContextMenuHighlight.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            await spectraController.ContextPlotSetHighlight(set);
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

        customPeaksToolStripMenuItem.Click += async (_, _) =>
        {
            if (customPeaksToolStripMenuItem.Checked)
            {
                customPeaksToolStripMenuItem.Checked = await processingController.SaveSpectraPeaks();
            }
            else
            {
                customPeaksToolStripMenuItem.Checked = await processingController.RemovedSpectraPeaks() is false;
            }
        };

        clearPeaksToolStripMenuItem.Click += async (_, _) => await processingController.ClearPeaks();

        plotView.SKControl!.MouseDoubleClick += async (_, _) =>
        {
            if (addPeaksToolStripMenuItem.Checked is false)
            {
                return;
            }

            var peak = new PeakData(
                center: coordinateProvider.Coordinates.X,
                amplitude: coordinateProvider.Coordinates.Y,
                halfWidth: 30f,
                gaussianContribution: 1f);

            await processingController.AddPeaks([peak]);
        };

        plotView.SKControl!.MouseDoubleClick += async (_, _) =>
        {
            if (removePeaksToolStripMenuItem.Checked is false)
            {
                return;
            }

            var peak = await peakController.TryGetPeak();

            if (peak is not null)
            {
                await processingController.RemovePeaks([peak.Peak]);
            }
        };

        plotStorageTreeView.NodeMouseDoubleClick += async (sender, _) =>
        {
            var node = TreeViewExtensions.GetClickTreeNode(sender);

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
        };

        plotContextMenuSmooth.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            await processingController.SmoothSpectras([plot.SpectraData]);
        };

        plotSetContextMenuSmooth.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            var spectras = set.Data.Select(d => d.SpectraData).ToArray();

            await processingController.SmoothSpectras(spectras);
        };

        fitPeaksToolStripMenuItem.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            await processingController.FitPeaks([plot.SpectraData]);
        };

        fitSetPeaksToolStripMenuItem.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            var spectras = set.Data.Select(d => d.SpectraData).ToArray();

            await processingController.FitPeaks(spectras);
        };
    }

#region SupportMethods

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
