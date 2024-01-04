using System.Data;
using Model.DataFormats.Base;
using Model.DataFormats.Spectras.Base;
using Model.DataSources.Base;
using Model.DataSources.Windows;
using Model.DataStorages;
using Model.DataStorages.Base;
using ScottPlot.Plottable;
using View.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Model.Controllers.Windows;
public class WinFormsController
{
    private readonly DataSource _source;
    private readonly DataStorage _storage;
    private DirectoryInfo _currentRoot;
    private ScottPlotContainer _plotContainer;
    private SortedSet<Data> _dataToBePloted;

    public DataSetNode ContextSet { get; set; }
    public Data ContextData { get; set; } = null!;
    public IPlottable? ContextPlot { get; set; }

    public event Action? OnRootChanged;
    public event Action? OnDataChanged;
    public event Action? OnPlotChanged;

    public WinFormsController(ScottPlot.FormsPlot plot)
    {
        _source = new WindowsFileSystem();
        _storage = new DirectoryBasedStorage(_source, "Storage");
        _currentRoot = new DirectoryInfo("E:\\ForProgramTest");

        _plotContainer = new(plot);
        _dataToBePloted = [];

        ContextSet = _storage.DefaultDataSet;
    }

    #region PlotMethodsMethods

    public async Task PlotContextAsync()
    {
        PrivateClearPlot();
        await AddContestToPlotAsync();
    }

    public async Task AddContestToPlotAsync()
    {
        foreach (var data in ContextSet.Data)
            _dataToBePloted.Add(data);
        await UpdatePlotAsync();
    }

    public async Task PlotDataAsync()
    {
        PrivateClearPlot();
        _dataToBePloted.Add(ContextData);
        await UpdatePlotAsync();
    }

    public async Task AddDataToPlotAsync(object dataItem)
    {
        if (dataItem is Data data)
        {
            _dataToBePloted.Add(data);
            await UpdatePlotAsync();
        }
    }

    public void ClearPlot()
    {
        PrivateClearPlot();
        OnPlotChanged?.Invoke();
    }

    public void ChangePlotVisibility(object plotItem, bool isVisible)
    {
        if (plotItem is Data data)
            _plotContainer.ChangeVisibility(data, isVisible);
    }

    public void SelectPlot(object plotItem)
    {
        if (plotItem is Data data)
            _plotContainer.SelectPlot(data);
    }

    private void PrivateClearPlot()
    {
        _dataToBePloted.Clear();
        _plotContainer.ClearPlot();
    }

    private async Task UpdatePlotAsync()
    {
        await _plotContainer.PlotData(_dataToBePloted);
        OnPlotChanged?.Invoke();
    }

    #endregion

    #region DataReadMethods

    public async Task RootReadAllAsync()
    {
        var setKey = $"{_currentRoot.Name} (all)";
        var rootSet = await Task.Run(() => new DirDataSet(setKey, _source, _currentRoot.FullName, true));
        _storage.AddDataSet(setKey, rootSet);
        OnDataChanged?.Invoke();
    }

    public async Task RootReadThisAsync()
    {
        var setKey = $"{_currentRoot.Name} (only this)";
        var rootSet = await Task.Run(() => new DirDataSet(setKey, _source, _currentRoot.FullName, false));
        _storage.AddDataSet(setKey, rootSet);
        OnDataChanged?.Invoke();
    }

    #endregion

    #region RootNavigationMethods

    public void RootSelect()
    {
        var selectedPath = SelectPathInExplorer();
        if (selectedPath != null)
            ChangeRoot(new DirectoryInfo(selectedPath));
    }

    public void RootStepBack()
    {
        if (_currentRoot.Parent != null)
            ChangeRoot(_currentRoot.Parent);
    }

    public void RootDoubleClick(object rootItem)
    {
        if (rootItem is FileInfo file)
        {
            var data = _source.ReadData(file.FullName);
            _storage.AddDataToDefaultSet(data);
            OnDataChanged?.Invoke();
            return;
        }

        if (rootItem is DirectoryInfo newRoot)
            ChangeRoot(newRoot);
    }

    private void ChangeRoot(DirectoryInfo newRoot)
    {
        _currentRoot = newRoot;
        OnRootChanged?.Invoke();
    }

    #endregion

    #region ConvertingMethods

    public async Task SaveAllSetAsESPAsync()
    {
        var path = SelectPathInExplorer();
        if (path is null) return;
        var output = new DirectoryInfo(Path.Combine(path, $"{ContextSet.Name} (converted all)"));
        await Task.Run(() => SaveAllSpectrasAs(ContextSet, output, ".esp"));
    }

    public async Task SaveThisSetAsESPAsync()
    {
        var path = SelectPathInExplorer();
        if (path is null) return;
        var output = new DirectoryInfo(Path.Combine(path, $"{ContextSet.Name} (converted only this)"));
        await Task.Run(() => SaveThisSpectrasAs(ContextSet, output, ".esp"));
    }

    public async Task SaveAsESP()
    {
        if (ContextData is not Spectra spectra) return;
        var path = SelectPathInExplorer();
        if (path is null) return;
        await Task.Run(() => SaveSpectraAs(spectra, new DirectoryInfo(path), ".esp"));
    }

    private void SaveAllSpectrasAs(DataSetNode root, DirectoryInfo outputRoot, string extension)
    {
        var track = LinkNodesAndOutputFolder(root, outputRoot);
        Parallel.ForEach(track.Keys, node => SaveThisSpectrasAs(node, track[node], extension));
    }

    private void SaveThisSpectrasAs(DataSetNode node, DirectoryInfo output, string extension)
    {
        var spectras = node.Data.Where(data => data is Spectra).Select(data => (Spectra)data);
        if (!output.Exists) output.Create();
        if (spectras.Any())
            Parallel.ForEach(spectras, spectra => SaveSpectraAs(spectra, output, extension));
    }

    private void SaveSpectraAs(Spectra spectra, DirectoryInfo output, string extension)
    {
        var fullName = Path.Combine(output.FullName, $"{spectra.Name}{extension}");
        _source.WriteData(spectra, fullName);
    }

    private Dictionary<DataSetNode, DirectoryInfo> LinkNodesAndOutputFolder(DataSetNode rootNode, DirectoryInfo outputRoot)
    {
        var track = new Dictionary<DataSetNode, DirectoryInfo> { [rootNode] = outputRoot };
        var queue = new Queue<DataSetNode>();
        queue.Enqueue(rootNode);
        while (queue.Count != 0)
        {
            var nodeInReference = queue.Dequeue();
            foreach (var nextNodeInReference in nodeInReference.Nodes)
            {
                DirectoryInfo inDestination = track[nodeInReference];
                string pathInDestination = Path.Combine(inDestination.FullName, nextNodeInReference.Name);
                DirectoryInfo nextInOutput = new(pathInDestination);
                track[nextNodeInReference] = nextInOutput;
                queue.Enqueue(nextNodeInReference);
            }
        }
        return track;
    }

    #endregion

    #region DeletingMethods

    public void DeleteSet()
    {
        if (ContextSet is null)
            throw new Exception("SelectedDataNode somehow is null");

        if (_storage.ContainsSet(ContextSet.Name) && ContextSet != _storage.DefaultDataSet)
            _storage.RemoveSet(ContextSet.Name);
        else
            ContextSet.DisconnectFromParent();

        OnDataChanged?.Invoke();
    }

    public void DeleteData()
    {
        if (ContextData is null)
            throw new Exception("SelectedData somehow is null");

        ContextSet.RemoveData(ContextData);
        OnDataChanged?.Invoke();
    }

    #endregion

    #region SupportMethods

    private string? SelectPathInExplorer()
    {
        using (FolderBrowserDialog dialog = new())
        {
            dialog.SelectedPath = _currentRoot.FullName;
            DialogResult result = dialog.ShowDialog();
            return result == DialogResult.OK ? dialog.SelectedPath : null;
        }
    }

    public IEnumerable<TreeNode> GetRootTree()
    {
        foreach (var dir in _currentRoot.GetDirectories())
            yield return new TreeNode { Text = dir.Name, Tag = dir, ImageIndex = 0 };
        foreach (var file in _currentRoot.GetFiles())
            yield return new TreeNode { Text = file.Name, Tag = file, ImageIndex = 1, };
    }

    public IEnumerable<TreeNode> GetDataTree()
    {
        foreach (var pair in _storage)
        {
            var node = new TreeNode { Text = pair.Key, Tag = pair.Value };
            ConnectDataSubnodes(node);
            yield return node;
        }
    }

    public IEnumerable<TreeNode> GetPlotTree() => _plotContainer.GetPlotNodes();

    private void ConnectDataSubnodes(TreeNode treeNode)
    {
        if (treeNode.Tag is not DataSetNode dataNode)
            throw new Exception(nameof(ConnectDataSubnodes));

        foreach (var child in dataNode.Nodes)
        {
            var subnode = new TreeNode
            {
                Text = child.Name,
                Tag = child,
            };
            ConnectDataSubnodes(subnode);
            treeNode.Nodes.Add(subnode);
        }

        foreach (var data in dataNode.Data)
        {
            var subnode = new TreeNode()
            {
                Text = data.Name,
                Tag = data,
            };
            treeNode.Nodes.Add(subnode);
        }
    }

    #endregion
}
