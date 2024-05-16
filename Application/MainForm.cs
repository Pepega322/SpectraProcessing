using Application.Controllers;
using Controllers.Interfaces;
using Domain.SpectraData;
using Domain.SpectraData.Processing;
using Domain.Storage;
using Microsoft.Extensions.DependencyInjection;
using Scott.Formats;

namespace Application;

public partial class MainForm : Form
{
	private readonly IDialogController dialogController;
	private readonly ICoordinateController coordinateController;
	private readonly IDataSourceController<Spectra> dataSourceController;
	private readonly IDataWriterController dataWriterController;
	private readonly ISpectraProcessingController processingController;
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
		processingController = provider.GetRequiredService<ISpectraProcessingController>();

		SetupSpectraProcessingController();
		SetupCoordinateControllerController();
		SetupDataStorageController();
		SetupPlotController();
		SetupDataReaderController();

		rootTree.NodeMouseClick += TreeNodeClickSelect;

		dataSetMenu.Tag = dataStorageTree;
		dataMenu.Tag = dataStorageTree;
		dataStorageTree.NodeMouseClick += TreeNodeClickSelect;
		dataStorageTree.NodeMouseClick += DataDrawContextMenu;
		dataStorageTree.NodeMouseClick += DataSetDrawContextMenu;

		plotSetMenu.Tag = plotStorageTree;
		plotMenu.Tag = plotStorageTree;
		plotStorageTree.NodeMouseClick += TreeNodeClickSelect;
		plotStorageTree.NodeMouseClick += PlotDrawContextMenu;
		plotStorageTree.NodeMouseClick += PlotSetDrawContextMenu;
		plotStorageTree.NodeMouseDoubleClick += TreeNodeClickSelect;

		_ = rootTree.BuildTreeAsync(dataSourceController.GetFileNodes);
	}

	private void SetupPlotController()
	{
		plotController.OnChange += async () => await plotStorageTree.BuildTreeAsync(plotController.GetPlotNodes);
		plotButtonClear.Click += (_, _) => plotController.PlotAreaClear();
		plotButtonResize.Click += (_, _) => plotController.PlotAreaResize();
		dataStorageTree.NodeMouseDoubleClick += async (_, e) =>
		{
			if (e.Button is MouseButtons.Left && e.Node.Tag is Spectra spectra)
				await plotController.DataAddToPlotAreaToDefault(spectra);
		};
		dataContextDataPlot.Click += async (sender, _) =>
		{
			var spectra = TreeViewHelpers.GetContextData<Spectra>(sender);
			await plotController.ContextDataAddToClearPlotToDefault(spectra);
		};
		dataContextDataSetPlot.Click += async (s, _) =>
		{
			var set = TreeViewHelpers.GetContextSet<Spectra>(s);
			await plotController.ContextDataAddToClearPlotArea(set);
		};
		plotContextPlotSetDelete.Click += async (sender, _) =>
		{
			var set = TreeViewHelpers.GetContextSet<SpectraPlot>(sender);
			await plotController.ContextPlotSetDelete(set);
		};
		plotContextPlotDelete.Click += async (sender, _) =>
		{
			var ownerSet = TreeViewHelpers.GetContextParentSet<SpectraPlot>(sender);
			var plot = TreeViewHelpers.GetContextData<SpectraPlot>(sender);
			await plotController.ContextPlotDelete(ownerSet, plot);
		};
		dataContextDataSetAddToPlot.Click += async (s, _) =>
		{
			var set = TreeViewHelpers.GetContextSet<Spectra>(s);
			await plotController.ContextDataSetAddToPlotArea(set);
		};
		plotStorageTree.NodeMouseDoubleClick += async (sender, _) =>
		{
			var node = TreeViewHelpers.GetClickTreeNode(sender);
			if (node is {Tag: SpectraPlot plot, Checked: true})
				await plotController.PlotHighlight(plot);
		};
		plotContextPlotSetHighlight.Click += async (sender, _) =>
		{
			var set = TreeViewHelpers.GetContextSet<SpectraPlot>(sender);
			await plotController.ContextPlotSetHighlight(set);
		};
		plotStorageTree.AfterCheck += async (_, e) =>
		{
			if (e.Node?.Tag is SpectraPlot plot)
				await plotController.ChangePlotVisibility(plot, e.Node.Checked);
		};
		plotStorageTree.AfterCheck += async (_, e) =>
		{
			if (e.Node?.Tag is DataSet<SpectraPlot> set)
				await plotController.ChangePlotSetVisibility(set, e.Node.Checked);
		};
	}

	private void SetupDataReaderController()
	{
		dataSourceController.OnChange += async () => await rootTree.BuildTreeAsync(dataSourceController.GetFileNodes);
		rootButtonStepBack.Click += (_, _) => dataSourceController.StepOutFolder();
		rootButtonRefresh.Click += (_, _) => dataSourceController.RefreshView();
		rootButtonSelect.Click += (_, _) =>
		{
			var path = dialogController.SelectPathInDialog();
			if (path is null) return;
			dataSourceController.ChangeFolder(path);
		};
		rootTree.NodeMouseDoubleClick += (_, e) =>
		{
			if (e.Node.Tag is DirectoryInfo newRoot)
				dataSourceController.ChangeFolder(newRoot.FullName);
		};
		rootButtonRead.Click += async (_, _) =>
		{
			var set = await dataSourceController.ReadFolderAsync();
			dataStorageController.AddDataSet(set);
		};
		rootButtonReadWithSubdirs.Click += async (_, _) =>
		{
			var set = await dataSourceController.ReadFolderFullDepthAsync();
			dataStorageController.AddDataSet(set);
		};
	}

	private void SetupDataStorageController()
	{
		dataStorageController.OnChange +=
			async () => await dataStorageTree.BuildTreeAsync(dataStorageController.GetDataNodes);
		dataButtonClear.Click += (_, _) => dataStorageController.Clear();
		dataContextDataSave.Click += async (sender, _) =>
		{
			var data = TreeViewHelpers.GetContextData<Spectra>(sender);
			var fullname = dialogController.SelectFullNameInDialog(data.Name, ".esp");
			if (fullname is null) return;
			await Task.Run(() => dataWriterController.DataWriteAs(data, fullname));
		};
		dataContextDataDelete.Click += (sender, _) =>
		{
			var ownerSet = TreeViewHelpers.GetContextParentSet<Spectra>(sender);
			var spectra = TreeViewHelpers.GetContextData<Spectra>(sender);
			dataStorageController.DeleteData(ownerSet, spectra);
		};
		dataContextDataSetSaveAs.Click += async (sender, _) =>
		{
			var path = dialogController.SelectPathInDialog();
			if (path is null) return;
			var set = TreeViewHelpers.GetContextSet<Spectra>(sender);
			var outputPath = Path.Combine(path, $"{set.Name} (converted)");
			await Task.Run(() => dataWriterController.SetOnlyWriteAs(set, outputPath, ".esp"));
		};
		dataContextDataSetAndSubsetsSaveAs.Click += async (sender, _) =>
		{
			var path = dialogController.SelectPathInDialog();
			if (path is null) return;
			var set = TreeViewHelpers.GetContextSet<Spectra>(sender);
			var outputPath = Path.Combine(path, $"{set.Name} (converted full depth)");
			await Task.Run(() => dataWriterController.SetFullDepthWriteAs(set, outputPath, ".esp"));
		};
		dataContextDataSetDelete.Click += (sender, _) =>
		{
			var set = TreeViewHelpers.GetContextSet<Spectra>(sender);
			dataStorageController.DeleteDataSet(set);
		};
	}

	private void SetupCoordinateControllerController()
	{
		coordinateController.OnChange += () =>
		{
			var c = coordinateController.Coordinates;
			mouseCoordinatesBox.Text = $@"X:{c.X: 0.00} {c.Y: 0.00}";
		};
		plotView.MouseMove += (_, e) => coordinateController.Coordinates = new Point<float>(e.X, e.Y);
	}

	private void SetupSpectraProcessingController()
	{
		plotButtonClearPeaks.Click += async (_, _) => await Task.Run(processingController.ClearBorders);
		plotContextPlotSetSubstractBaseLine.Click += async (sender, _) =>
		{
			var set = TreeViewHelpers.GetContextSet<SpectraPlot>(sender);
			var spectra = set.Select(s => s.Spectra).ToArray();
			var substracted = await processingController.SubstractBaseline(spectra);
			var substractedSet = new DataSet<Spectra>($"{set.Name} b-", substracted);
			dataStorageController.AddDataSet(substractedSet);
		};
		plotContextPlotSubstractBaseLine.Click += async (sender, _) =>
		{
			var plot = TreeViewHelpers.GetContextData<SpectraPlot>(sender);
			var substracted = await processingController.SubstractBaseline(plot.Spectra);
			dataStorageController.AddDataToDefaultSet(substracted);
		};
		plotButtonAddPeak.Click += async (_, _) =>
		{
			var start = await coordinateController.GetCoordinateByClick();
			var end = await coordinateController.GetCoordinateByClick();
			var border = new PeakBorder(start.X, end.X);
			processingController.AddBorder(border);
			plotView.Refresh();
		};
		plotButtonDeleteLastPeak.Click += async (_, _) =>
		{
			var last = processingController.Borders.LastOrDefault();
			if (last != default)
			{
				await Task.Run(() => processingController.RemoveBorder(last));
			}

			plotView.Refresh();
		};
		plotContextPlotSetPeaksProcess.Click += async (sender, _) =>
		{
			var plotSet = TreeViewHelpers.GetContextSet<SpectraPlot>(sender);
			var fullname = dialogController.SelectFullNameInDialog(plotSet.Name, ".txt");
			if (fullname is null) return;
			var spectraSet = new DataSet<Spectra>(plotSet.Name, plotSet.Select(plot => plot.Spectra));
			var processed = await processingController.ProcessPeaksForSpectraSet(spectraSet);
			dataWriterController.DataWriteAs(processed, fullname);
		};
		plotContextPlotPeaksProcess.Click += async (sender, _) =>
		{
			var plot = TreeViewHelpers.GetContextData<SpectraPlot>(sender);
			var fullname = dialogController.SelectFullNameInDialog(plot.Name, ".txt");
			if (fullname is null) return;
			var processed = await processingController.ProcessPeaksForSingleSpectra(plot.Spectra);
			dataWriterController.DataWriteAs(processed, fullname);
		};
		plotContextPlotSetAverageSpectra.Click += async (s, _) =>
		{
			var set = TreeViewHelpers.GetContextSet<SpectraPlot>(s);
			var spectra = set.Select(plot => plot.Spectra).ToArray();
			var average = await processingController.GetAverageSpectra(spectra);
			average.Name = $"{set.Name} (average)";
			dataStorageController.AddDataToDefaultSet(average);
		};
	}

#region SupportMethods

	private static void TreeNodeClickSelect(object? sender, TreeNodeMouseClickEventArgs e)
	{
		if (sender is TreeView treeView)
			treeView.SelectedNode = e.Node;
	}

	private void PlotSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
	{
		if (e.Button is MouseButtons.Right && e.Node.Tag is DataSet<SpectraPlot>)
			e.Node.ContextMenuStrip = plotSetMenu;
	}

	private void PlotDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
	{
		if (e.Button is MouseButtons.Right && e.Node.Tag is SpectraPlot)
			e.Node.ContextMenuStrip = plotMenu;
	}

	private void DataSetDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
	{
		if (e.Button is MouseButtons.Right && e.Node.Tag is DataSet<Spectra>)
			e.Node.ContextMenuStrip = dataSetMenu;
	}

	private void DataDrawContextMenu(object? sender, TreeNodeMouseClickEventArgs e)
	{
		if (e.Button is MouseButtons.Right && e.Node.Tag is Spectra)
			e.Node.ContextMenuStrip = dataMenu;
	}

#endregion
}