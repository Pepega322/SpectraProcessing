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
    private readonly IPeakProcessingController peakProcessingController;
    private readonly ISpectraProcessingController spectraProcessingController;

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
        peakProcessingController = provider.GetRequiredService<IPeakProcessingController>();
        spectraProcessingController = provider.GetRequiredService<ISpectraProcessingController>();

        SetupDataReaderController();
        SetupDataStorageController();
        SetupPlotController();
        SetupCoordinateProvider();
        SetupSpectraProcessingController();
        SetupPeakProcessingController();

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

        tempToolStripMenuItem.Click +=  (_, _) =>
        {
            spectraController.HighlightedData?.SpectraData.Points
                .Transform((_, y) => y + (float) numericUpDown1.Value);
        };
    }

    private async Task<int> Prepare()
    {
        var set = await spectraDataProvider.ReadFolderAsync("d:\\Study\\Chemfuck\\диплом\\DEV\\");

        await dataStorageProvider.AddDataSet(new StringKey(set.Name), set);

        var spectra = set.Data.Single(s => s.Name == "Gauss.esp");

        await spectraController.AddDataToPlotToDefault(spectra);

        await spectraController.PlotResize();

        plotView.Refresh();

        return 1;
    }

    private void SetupDataReaderController()
    {
        readFolderToolStripMenuItem.Click += async (_, _) =>
        {
            // await Prepare();
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = await spectraDataProvider.ReadFolderAsync(path);

            await dataStorageProvider.AddDataSet(new StringKey(set.Name), set);
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

        dataContextMenuDelete.Click += async (sender, _) =>
        {
            var ownerSet = TreeViewExtensions.GetContextParentSet<SpectraData>(sender);
            var spectra = TreeViewExtensions.GetContextData<SpectraData>(sender);
            await dataStorageProvider.DeleteData(ownerSet!, spectra!);
        };

        dataSetContextMenuClear.Click += (_, _) => dataStorageProvider.Clear();

        dataSetContextMenuDelete.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraData>(sender);
            await dataStorageProvider.DeleteDataSet(set!);
        };

        //Plotting
        dataContextMenuPlot.Click += async (sender, _) =>
        {
            var spectra = TreeViewExtensions.GetContextData<SpectraData>(sender);
            await spectraController.AddDataToClearPlotToDefault(spectra!);
        };

        dataSetContextMenuPlot.Click += async (s, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraData>(s);
            await spectraController.AddDataSetToClearPlot(set!);
        };

        dataSetContextMenuAddToPlot.Click += async (s, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraData>(s);
            await spectraController.AddDataSetToPlot(set!);
        };

        dataStorageTreeView.NodeMouseDoubleClick += async (_, e) =>
        {
            if (e is { Button: MouseButtons.Left, Node.Tag: SpectraData spectra })
            {
                await spectraController.AddDataToPlotToDefault(spectra);
            }
        };
    }

    private void SetupPlotController()
    {
        resizeToolStripMenuItem.Click += async (_, _) => await spectraController.PlotResize();

        spectraController.OnPlotStorageChanged +=
            async () => await plotStorageTreeView.BuildTreeAsync(spectraController.GetPlotNodes);

        spectraController.OnPlotAreaChanged += () => plotView.Refresh();

        plotContextMenuSaveAsEsp.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            var fullname = dialogController.GetSaveFileFullName(plot!.Name, ".esp");

            if (fullname is null)
            {
                return;
            }

            await spectraDataProvider.DataWriteAs(plot.SpectraData, fullname);
        };

        plotContextMenuDelete.Click += async (sender, _) =>
        {
            var ownerSet = TreeViewExtensions.GetContextParentSet<SpectraDataPlot>(sender);

            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            await spectraController.ErasePlot(ownerSet!, plot!);

            await peakProcessingController.RemovePeaks(plot!.SpectraData);

            await spectraController.PlotRemoveHighlight(plot);
        };

        plotContextMenuClear.Click += async (_, _) =>
        {
            await spectraController.PlotClear();
            await peakProcessingController.ClearPeaks();
        };

        plotStorageTreeView.AfterCheck += async (_, e) =>
        {
            switch (e.Node?.Tag)
            {
                case SpectraDataPlot plot:
                    await spectraController.ChangePlotVisibility(plot, e.Node.Checked);

                    var isPlotHighlighted = await spectraController.IsPlotHighlighted(plot);

                    if (isPlotHighlighted)
                    {
                        await ChangePlotCheckout(plot, e.Node.Checked);
                    }

                    break;

                case DataSet<SpectraDataPlot> set:
                    await spectraController.ChangePlotSetVisibility(set, e.Node.Checked);

                    var highlightedPlot = set.Data.FirstOrDefault(p => spectraController.IsPlotHighlighted(p).Result);

                    if (highlightedPlot is not null)
                    {
                        await ChangePlotCheckout(highlightedPlot, e.Node.Checked);
                    }

                    break;
            }
        };

        plotSetContextMenuSaveAsEsp.Click += async (sender, _) =>
        {
            var path = dialogController.GetFolderPath();

            if (path is null)
            {
                return;
            }

            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            // var outputPath = Path.Combine(path, $"{set!.Name}");

            var spectrasToSave = set!.Data
                .Select(plot => plot.SpectraData)
                .ToArray();

            await spectraDataProvider.DataWriteAs(spectrasToSave, path, ".esp");
        };

        plotSetContextMenuDelete.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);
            await spectraController.ErasePlotSet(set!);

            foreach (var plot in set!.Data)
            {
                await peakProcessingController.RemovePeaks(plot.SpectraData);
                await spectraController.PlotRemoveHighlight(plot);
            }
        };

        plotSetContextMenuClear.Click += async (_, _) =>
        {
            await spectraController.PlotClear();
            await peakProcessingController.ClearPeaks();
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
            mouseCoordinatesBox.Text = $@"X:{c.X:0.00} Y:{c.Y:0.00}";
        };
    }

    private void SetupPeakProcessingController()
    {
        peakProcessingController.OnPlotAreaChanged += plotView.Refresh;
        peakController.OnPeakChanges += plotView.Refresh;

        plotView.MouseDown += async (_, _) => await peakController.TryMovePeak();

        customPeaksToolStripMenuItem.Click += async (_, _) =>
        {
            if (customPeaksToolStripMenuItem.Checked)
            {
                customPeaksToolStripMenuItem.Checked = await peakProcessingController.SaveCurrentSpectraPeaks();
            }
            else
            {
                customPeaksToolStripMenuItem.Checked =
                    await peakProcessingController.RemoveCurrentSpectraPeaks() is false;
            }
        };

        clearPeaksToolStripMenuItem.Click += async (_, _) => await peakProcessingController.ClearCurrentSpectraPeaks();

        plotView.SKControl!.MouseDoubleClick += async (_, _) =>
        {
            if (addOrRemovePeaksToolStripMenuItem.Checked is false)
            {
                return;
            }

            var peak = await peakController.TryGetPeak();

            if (peak is not null)
            {
                await peakProcessingController.RemovePeaksForCurrentSpectra([peak.Peak]);
            }
            else
            {
                await peakProcessingController.AddPeaksForCurrentSpectra(
                [
                    new PeakData(
                        center: coordinateProvider.Coordinates.X,
                        amplitude: coordinateProvider.Coordinates.Y,
                        halfWidth: (float) plotView.Plot.Axes.Bottom.Width * 0.025f,
                        gaussianContribution: 0.8f),
                ]);
            }
        };

        plotStorageTreeView.NodeMouseDoubleClick += async (sender, _) =>
        {
            var node = TreeViewExtensions.GetClickTreeNode(sender);

            await CheckoutPlotNode(node!);
        };

        plotContextMenuFitPeaks.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            await peakProcessingController.FitPeaks([plot!.SpectraData]);
        };

        plotSetContextMenuFitPeaks.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            var spectras = set!.Data.Select(d => d.SpectraData).ToArray();

            await peakProcessingController.FitPeaks(spectras);
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

            var peaks = peakProcessingController.CurrentPeaks
                .Select(p => p.Peak)
                .ToArray();

            var set = new PeakDataSet(peaks, name);

            await peakDataProvider.DataWriteAs(set, fullname);
        };

        plotContextMenuExportPeaks.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            var peaks = await peakProcessingController.ExportPeaks(plot!.SpectraData);

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
                var peaks = await peakProcessingController.ExportPeaks(plot.SpectraData);

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

            await peakProcessingController.AddPeaksForCurrentSpectra(set.Peaks);
        };

        plotContextMenuImportPeaks.Click += async (sender, _) =>
        {
            var fullName = dialogController.GetReadFileFullName();

            if (fullName is null)
            {
                return;
            }

            var peaksSet = await peakDataProvider.ReadDataAsync(fullName);

            if (peaksSet is null)
            {
                return;
            }

            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender, out var node);

            await peakProcessingController.ImportPeaks(plot!.SpectraData, peaksSet.Peaks);

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
                await peakProcessingController.ImportPeaks(spectraData, peaksSet.Peaks);
            }
        };
    }

    private void SetupSpectraProcessingController()
    {
        spectraProcessingController.OnPlotAreaChanged += plotView.Refresh;

        numericUpDown1.Minimum = 1;
        numericUpDown1.Maximum = decimal.MaxValue;
        numericUpDown1.Increment = 1;
        numericUpDown1.DecimalPlaces = 2;

        numericUpDown1.Value = 100;
        spectraProcessingController.CurrentWidth = numericUpDown1.Value;
        numericUpDown1.ValueChanged += (_, _) => spectraProcessingController.CurrentWidth = numericUpDown1.Value;


        plotContextMenuSmooth.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            await spectraProcessingController.SmoothSpectras([plot!.SpectraData]);
        };

        plotSetContextMenuSmooth.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            var spectras = set!.Data.Select(d => d.SpectraData).ToArray();

            await spectraProcessingController.SmoothSpectras(spectras);
        };

        baselineModeToolStripMenuItem.Click += async (_, _) =>
        {
            var isBaselineModeOn = !baselineModeToolStripMenuItem.Checked;

            baselineModeToolStripMenuItem.Checked = isBaselineModeOn;

            var plot = TreeViewExtensions.GetClickData<SpectraDataPlot>(plotContextMenu.Tag);

            if (plot is null)
            {
                return;
            }

            if (isBaselineModeOn)
            {
                await spectraProcessingController.DrawBaseline(plot!.SpectraData);
            }
            else
            {
                await spectraProcessingController.ClearBaseline();
            }
        };

        plotContextMenuSubstractBaseline.Click += async (sender, _) =>
        {
            var plot = TreeViewExtensions.GetContextData<SpectraDataPlot>(sender);

            await spectraProcessingController.SubstractBaseline([plot!.SpectraData]);
        };

        plotSetContextMenuSubstractBaseline.Click += async (sender, _) =>
        {
            var set = TreeViewExtensions.GetContextSet<SpectraDataPlot>(sender);

            var spectras = set!.Data
                .Select(p => p.SpectraData)
                .ToArray();

            await spectraProcessingController.SubstractBaseline(spectras);
        };
    }

#region SupportMethods

    private async Task CheckoutPlotNode(TreeNode node)
    {
        if (node is not { Tag: SpectraDataPlot plot, Checked: true })
        {
            return;
        }

        var isHighlighted = await spectraController.PlotHighlight(plot);

        await ChangePlotCheckout(plot, isHighlighted);

        if (isHighlighted)
        {
            await HighlightNodeUntilNextClick();
        }

        return;

        async Task HighlightNodeUntilNextClick()
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
    }

    private async Task ChangePlotCheckout(SpectraDataPlot plot, bool isChecked)
    {
        if (isChecked)
        {
            customPeaksToolStripMenuItem.Checked = await peakProcessingController.CheckoutSpectra(plot.SpectraData);

            if (baselineModeToolStripMenuItem.Checked)
            {
                await spectraProcessingController.DrawBaseline(plot.SpectraData);
            }
        }
        else
        {
            customPeaksToolStripMenuItem.Checked = await peakProcessingController.CheckoutSpectra(null);

            if (baselineModeToolStripMenuItem.Checked)
            {
                await spectraProcessingController.ClearBaseline();
            }
        }
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
