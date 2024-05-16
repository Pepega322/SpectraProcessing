using System.Collections.Concurrent;
using Controllers.Interfaces;
using Domain.Graphics;
using Domain.SpectraData;
using Domain.SpectraData.Processing;
using Domain.Storage;
using Scott.Formats;
using ScottPlot.WinForms;

namespace Application.Controllers;

public class WinformsMainController(
	FormsPlot form,
	ICoordinateController coordinateController,
	IDialogController dialogController,
	IDataReaderController<Spectra> dataReaderController,
	IDataWriterController dataWriterController,
	IDataStorageController<Spectra> dataStorageController,
	IPlotBuilder<Spectra, SpectraPlot> plotBuilder,
	IDataStorageController<SpectraPlot> plotStorageController,
	IGraphicsController<SpectraPlot> graphicsController,
	ISpectraProcessingController spectraProcessingController
)
{
	public event Action? OnDataChanged;
	public event Action? OnPlotChanged;
	public event Action? OnRootChanged;
	public event Action? OnPlotMouseCoordinatesChanged;
	public event Action? OnPlotClear = () =>
	{
		plotStorageController.Clear();
		graphicsController.ClearArea();
		spectraProcessingController.RedrawBorders();
	};

	public Point<float> PlotCoordinates => coordinateController.Coordinates;

#region PlotControllerMethods

	public async Task ContextDataSetAddToPlotArea(object? sender, EventArgs e)
	{
		var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
		await Task.Run(() =>
		{
			var plots = new ConcurrentBag<SpectraPlot>();
			Parallel.ForEach(set, data =>
			{
				var plt = plotBuilder.GetPlot(data);
				plots.Add(plt);
			});
			var plotSet = new DataSet<SpectraPlot>(set.Name, plots);
			plotStorageController.AddDataSet(plotSet);
			graphicsController.DrawDataSet(plotSet);
		});
		form.Refresh();
		OnPlotChanged?.Invoke();
	}

	public async Task ContextDataAddToClearPlotArea(object? sender, EventArgs e)
	{
		plotStorageController.Clear();
		graphicsController.ClearArea();
		OnPlotClear?.Invoke();
		await ContextDataSetAddToPlotArea(sender, e);
	}

	public async Task DataAddToPlotAreaToDefault(object? sender, TreeNodeMouseClickEventArgs e)
	{
		if (e.Button is MouseButtons.Left && e.Node.Tag is Spectra spectra)
		{
			await Task.Run(() =>
			{
				var plot = plotBuilder.GetPlot(spectra);
				plotStorageController.AddDataToDefaultSet(plot);
				graphicsController.DrawData(plot);
			});
			form.Refresh();
			OnPlotChanged?.Invoke();
		}
	}

	public async Task ContextDataAddToClearPlotToDefault(object? sender, EventArgs e)
	{
		var spectra = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
		await Task.Run(() =>
		{
			plotStorageController.Clear();
			var plot = plotBuilder.GetPlot(spectra);
			plotStorageController.AddDataToDefaultSet(plot);
			graphicsController.ClearArea();
			graphicsController.DrawData(plot);
		});
		form.Refresh();
		OnPlotChanged?.Invoke();
	}

	public async Task ChangePlotSetVisibility(object? sender, TreeViewEventArgs e)
	{
		if (e.Node?.Tag is DataSet<SpectraPlot> set)
		{
			await Task.Run(() => graphicsController.ChangeDataSetVisibility(set, e.Node.Checked));
			form.Refresh();
			OnPlotChanged?.Invoke();
		}
	}

	public async Task ChangePlotVisibility(object? sender, TreeViewEventArgs e)
	{
		if (e.Node?.Tag is SpectraPlot plot)
		{
			await Task.Run(() => graphicsController.ChangeDataVisibility(plot, e.Node.Checked));
			form.Refresh();
		}
	}

	public async Task ContextPlotSetHighlight(object? sender, EventArgs e)
	{
		var set = WinformsTreeViewHelpers.GetContextSet<SpectraPlot>(sender);
		await Task.Run(() => graphicsController.HighlightDataSet(set));
		form.Refresh();
	}

	public async Task PlotHighlight(object? sender, TreeNodeMouseClickEventArgs e)
	{
		var node = WinformsTreeViewHelpers.GetClickTreeNode(sender);
		if (node is {Tag: SpectraPlot plot, Checked: true})
		{
			await Task.Run(() => graphicsController.HighlightData(plot));
			form.Refresh();
		}
	}

	public async Task ContextPlotSetDelete(object? sender, EventArgs e)
	{
		var set = WinformsTreeViewHelpers.GetContextSet<SpectraPlot>(sender);
		await Task.Run(() =>
		{
			plotStorageController.DeleteDataSet(set);
			graphicsController.EraseDataSet(set);
		});
		form.Refresh();
		OnPlotChanged?.Invoke();
	}

	public async Task ContextPlotDelete(object? sender, EventArgs e)
	{
		var plot = WinformsTreeViewHelpers.GetContextData<SpectraPlot>(sender);
		var ownerSet = WinformsTreeViewHelpers.GetContextParentSet<SpectraPlot>(sender);
		await Task.Run(() =>
		{
			plotStorageController.DeleteData(ownerSet, plot);
			graphicsController.EraseData(plot);
		});
		form.Refresh();
		OnPlotChanged?.Invoke();
	}

	public async Task ContextPlotSetPeaksProcess(object? sender, EventArgs e)
	{
		var set = WinformsTreeViewHelpers.GetContextSet<SpectraPlot>(sender);
		var fullname = dialogController.SelectFullNameInDialog(set.Name, ".txt");
		if (fullname is not null)
		{
			await Task.Run(() =>
			{
				var spectraSet = new DataSet<Spectra>(set.Name, set.Select(plot => plot.Spectra));
				var info = spectraProcessingController.ProcessPeaksForSpectraSet(spectraSet);
				dataWriterController.DataWriteAs(info, fullname);
			});
		}
	}

	public async Task ContextPlotPeaksProcess(object? sender, EventArgs e)
	{
		var plot = WinformsTreeViewHelpers.GetContextData<SpectraPlot>(sender);
		var fullname = dialogController.SelectFullNameInDialog(plot.Name, ".txt");
		if (fullname is not null)
		{
			await Task.Run(() =>
			{
				var info = spectraProcessingController.ProcessPeaksForSingleSpectra(plot.Spectra);
				dataWriterController.DataWriteAs(info, fullname);
			});
		}
	}

	public async Task AddPeakBordersToPlotArea(object? sender, EventArgs e)
	{
		await Task.Run(async () =>
		{
			var start = await coordinateController.GetCoordinateByClick();
			var end = await coordinateController.GetCoordinateByClick();
			var border = new PeakBorder(start.X, end.X);
			spectraProcessingController.AddBorder(border);
		});
		form.Refresh();
	}

	public async Task DeleteLastPeakBordersFromPlotArea(object? sender, EventArgs e)
	{
		var last = spectraProcessingController.Borders.LastOrDefault();
		if (last != default)
		{
			await Task.Run(() => spectraProcessingController.RemoveBorder(last));
		}

		form.Refresh();
	}

	public async Task PlotClearPeakBorders(object? sender, EventArgs e)
	{
		await Task.Run(spectraProcessingController.ClearBorders);
		form.Refresh();
	}

	public void SetPlotCoordinates(object? sender, MouseEventArgs e)
	{
		var coordinates = form.Plot.GetCoordinates(e.X, e.Y);
		coordinateController.Coordinates = new Point<float>((float) coordinates.X, (float) coordinates.Y);
		OnPlotMouseCoordinatesChanged?.Invoke();
	}

	public void PlotAreaClear()
	{
		plotStorageController.Clear();
		graphicsController.ClearArea();
		spectraProcessingController.RedrawBorders();
		OnPlotChanged?.Invoke();
		form.Refresh();
	}

	public void PlotAreaResize()
	{
		graphicsController.ResizeArea();
		form.Refresh();
	}

	public IEnumerable<TreeNode> PlotGetTree()
	{
		foreach (var pair in plotStorageController.StorageRecords)
		{
			var setNode = new TreeNode
			{
				Text = pair.Key,
				Tag = pair.Value,
				Checked = false
			};
			foreach (var plot in pair.Value.OrderByDescending(p => p.Name))
			{
				var subnode = new TreeNode
				{
					Text = plot.Name,
					Tag = plot,
					Checked = plot.GetPlottables().First().IsVisible
				};
				if (subnode.Checked) setNode.Checked = true;
				setNode.Nodes.Add(subnode);
			}

			if (setNode.Nodes.Count > 0)
				yield return setNode;
		}
	}

#endregion

#region DataControllerMethods

	public async Task ContextDataSetAndSubsetsSaveAsEspAsync(object? sender, EventArgs e)
	{
		var path = dialogController.SelectPathInDialog();
		if (path is not null)
		{
			await Task.Run(() =>
			{
				var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
				var outputPath = Path.Combine(path, $"{set.Name} (converted full depth)");
				dataWriterController.SetFullDepthWriteAs(set, outputPath, ".esp");
			});
		}
	}

	public async Task ContextDataSetSaveAsEspAsync(object? sender, EventArgs e)
	{
		var path = dialogController.SelectPathInDialog();
		if (path is not null)
		{
			await Task.Run(() =>
			{
				var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
				var outputPath = Path.Combine(path, $"{set.Name} (converted)");
				dataWriterController.SetOnlyWriteAs(set, outputPath, ".esp");
			});
		}
	}

	public async Task ContextDataSaveAsEspAsync(object? sender, EventArgs e)
	{
		var data = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
		var fullname = dialogController.SelectFullNameInDialog(data.Name, ".esp");
		if (fullname is not null)
		{
			await Task.Run(() => dataWriterController.DataWriteAs(data, fullname));
		}
	}

	public async Task ContextSetFullDepthSubstractBaselineAsync(object? sender, EventArgs e)
	{
		var root = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
		var substractedSet =
			await Task.Run(() => spectraProcessingController.SubstractBaselineForSpectraSetFullDepth(root));
		dataStorageController.AddDataSet(substractedSet);
		OnDataChanged?.Invoke();
	}

	public async Task ContextSetOnlySubstractBaselineAsync(object? sender, EventArgs e)
	{
		var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
		var substractedSet =
			await Task.Run(() => spectraProcessingController.SubstractBaselineForSingleSpectraSet(set));
		dataStorageController.AddDataSet(substractedSet);
		OnDataChanged?.Invoke();
	}

	public async Task ContextDataSubstractBaselineAsync(object? sender, EventArgs e)
	{
		var data = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
		var substracted = await Task.Run(() => spectraProcessingController.SubstractBaselineForSingleSpectra(data));
		dataStorageController.AddDataToDefaultSet(substracted);
		OnDataChanged?.Invoke();
	}

	public async Task ContextSetOnlyGetAverageSpectra(object? sender, EventArgs e)
	{
		var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
		var average = await Task.Run(() => spectraProcessingController.GetAverageSpectraForSet(set));
		dataStorageController.AddDataToDefaultSet(average);
		OnDataChanged?.Invoke();
	}

	public void ContextDataSetDelete(object? sender, EventArgs e)
	{
		var set = WinformsTreeViewHelpers.GetContextSet<Spectra>(sender);
		dataStorageController.DeleteDataSet(set);
		OnDataChanged?.Invoke();
	}

	public void ContextDataDelete(object? sender, EventArgs e)
	{
		var spectra = WinformsTreeViewHelpers.GetContextData<Spectra>(sender);
		var owner = WinformsTreeViewHelpers.GetContextParentSet<Spectra>(sender);
		if (dataStorageController.DeleteData(owner, spectra))
			OnDataChanged?.Invoke();
	}

	public void DataClear()
	{
		dataStorageController.Clear();
		OnDataChanged?.Invoke();
	}

	public IEnumerable<TreeNode> DataGetTree()
	{
		foreach (var pair in dataStorageController.StorageRecords)
		{
			var node = new TreeNode {Text = pair.Key, Tag = pair.Value};
			ConnectDataSubnodes(node);
			yield return node;
		}
	}

	private void ConnectDataSubnodes(TreeNode node)
	{
		if (node.Tag is not DataSet<Spectra> set)
			throw new Exception(nameof(ConnectDataSubnodes));

		foreach (var child in set.Subsets.OrderByDescending(child => child.Name))
		{
			var subnode = new TreeNode
			{
				Text = child.Name,
				Tag = child,
			};
			ConnectDataSubnodes(subnode);
			node.Nodes.Add(subnode);
		}

		foreach (var data in set.OrderByDescending(data => data.Name))
		{
			var subnode = new TreeNode()
			{
				Text = data.Name,
				Tag = data,
			};
			node.Nodes.Add(subnode);
		}
	}

#endregion

#region DirectoryControllerMethods

	public async Task RootReadFullDepthAsync(object? sender, EventArgs e)
	{
		var set = await Task.Run(dataReaderController.ReadRootFullDepth);
		dataStorageController.AddDataSet(set);
		OnDataChanged?.Invoke();
	}

	public async Task RootReadAsync(object? sender, EventArgs e)
	{
		var set = await Task.Run(dataReaderController.ReadRoot);
		dataStorageController.AddDataSet(set);
		OnDataChanged?.Invoke();
	}

	public void RootSelect(object? sender, EventArgs e)
	{
		var selectedPath = dialogController.SelectPathInDialog();
		if (selectedPath != null && dataReaderController.ChangeRoot(selectedPath))
			OnRootChanged?.Invoke();
	}

	public void RootStepBack(object? sender, EventArgs e)
	{
		if (dataReaderController.StepBack())
			OnRootChanged?.Invoke();
	}

	public void RootRefresh(object? sender, EventArgs e)
	{
		OnRootChanged?.Invoke();
	}

	public async Task RootDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
	{
		if (e.Node.Tag is FileInfo file)
		{
			var data = await Task.Run(() => dataReaderController.Read(file.FullName));
			if (data != null && dataStorageController.AddDataToDefaultSet(data))
				OnDataChanged?.Invoke();
		}
		else if (e.Node.Tag is DirectoryInfo newRoot)
		{
			if (dataReaderController.ChangeRoot(newRoot.FullName))
				OnRootChanged?.Invoke();
		}
	}

	public IEnumerable<TreeNode> RootGetTree()
	{
		foreach (var dir in dataReaderController.Root.GetDirectories().OrderByDescending(d => d.Name))
		{
			yield return new TreeNode
			{
				Text = dir.Name,
				Tag = dir,
				ImageIndex = 0
			};
		}

		foreach (var file in dataReaderController.Root.GetFiles().OrderByDescending(f => f.Name))
		{
			yield return new TreeNode
			{
				Text = file.Name,
				Tag = file,
				ImageIndex = 1
			};
		}
	}

#endregion
}