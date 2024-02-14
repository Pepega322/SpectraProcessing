using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;
using ScottPlot;
using ScottPlot.WinForms;

namespace View.Controllers;
public class WinFormsController {
    private WorkingDirectory workingDirectory;

    private DataSource source;
    private DataStorage storage;
    private ScottPlotController plotContainer;
    private SortedSet<Data> dataToBePloted;

    public DataSetNode ContextSet { get; set; }
    public Data ContextData { get; set; } = null!;
    public IPlottable? ContextPlot { get; set; }

    public event Action? OnDataChanged;
    public event Action? OnPlotChanged;
    public event Action? OnRootChanged;

    public WinFormsController(FormsPlot plot) {
        source = new WindowsFileSystem();
        storage = new DirectoryBasedStorage(source, "Storage");
        workingDirectory = new WorkingDirectory("D:\\Study\\Chemfuck\\Lab\\MixturesData");
        plotContainer = new(plot);
        dataToBePloted = [];
        ContextSet = storage.DefaultDataSet;
    }

    #region PlotMethods

    public async Task PlotContextAsync() {
        PrivateClearPlot();
        await AddContestToPlotAsync();
    }

    public async Task AddContestToPlotAsync() {
        foreach (var data in ContextSet.Data)
            dataToBePloted.Add(data);
        await UpdatePlotAsync();
    }

    public async Task PlotDataAsync() {
        PrivateClearPlot();
        dataToBePloted.Add(ContextData);
        await UpdatePlotAsync();
    }

    public async Task AddDataToPlotAsync(object dataItem) {
        if (dataItem is Data data) {
            dataToBePloted.Add(data);
            await UpdatePlotAsync();
        }
    }

    public void ClearPlot() {
        PrivateClearPlot();
        OnPlotChanged?.Invoke();
    }

    public void ChangePlotVisibility(object plotItem, bool isVisible) {
        if (plotItem is Data data)
            plotContainer.ChangeVisibility(data, isVisible);
    }

    public void SelectPlot(object plotItem) {
        if (plotItem is Data data)
            plotContainer.SelectPlot(data);
    }

    private void PrivateClearPlot() {
        dataToBePloted.Clear();
        plotContainer.Clear();
    }

    private async Task UpdatePlotAsync() {
        await plotContainer.PlotSet(dataToBePloted);
        OnPlotChanged?.Invoke();
    }

    #endregion

    #region WorkingDirectoryMethods

    public async Task RootReadAllAsync() {
        await workingDirectory.ReadWithSubdirectoriesAsync(storage);
        OnDataChanged?.Invoke();
    }

    public async Task RootReadThisAsync() {
        await workingDirectory.ReadAsync(storage);
        OnDataChanged?.Invoke();
    }

    public void RootSelect() {
        var selectedPath = SelectPathInExplorer();
        if (selectedPath != null)
            workingDirectory.ChangeDirectory(selectedPath);
        OnRootChanged?.Invoke();
    }

    public void RootStepBack() {
        workingDirectory.StepBack();
        OnRootChanged?.Invoke();
    }

    public void RootDoubleClick(object rootItem) {
        if (rootItem is FileInfo file) {
            var data = source.ReadData(file.FullName);
            if (storage.AddToDefaultSet(data))
                OnDataChanged?.Invoke();
        }

        if (rootItem is DirectoryInfo newRoot) {
            workingDirectory.ChangeDirectory(newRoot.FullName);
            OnRootChanged?.Invoke();
        }
    }

    #endregion

    #region ConvertingMethods

    public async Task SaveAllSetAsESPAsync() {
        var path = SelectPathInExplorer();
        if (path is null) return;
        var output = new DirectoryInfo(Path.Combine(path, $"{ContextSet.Name} (converted all)"));
        await Task.Run(() => SaveAllSpectrasAs(ContextSet, output, ".esp"));
    }

    public async Task SaveThisSetAsESPAsync() {
        var path = SelectPathInExplorer();
        if (path is null) return;
        var output = new DirectoryInfo(Path.Combine(path, $"{ContextSet.Name} (converted only this)"));
        await Task.Run(() => SaveThisSpectrasAs(ContextSet, output, ".esp"));
    }

    public async Task SaveAsESPAsync() {
        if (ContextData is not Spectra spectra) return;
        var path = SelectPathInExplorer();
        if (path is null) return;
        await Task.Run(() => SaveSpectraAs(spectra, new DirectoryInfo(path), ".esp"));
    }

    private void SaveAllSpectrasAs(DataSetNode root, DirectoryInfo outputRoot, string extension) {
        var track = LinkNodesAndOutputFolder(root, outputRoot);
        Parallel.ForEach(track.Keys, node => SaveThisSpectrasAs(node, track[node], extension));
    }

    private void SaveThisSpectrasAs(DataSetNode node, DirectoryInfo output, string extension) {
        var spectras = node.Data.Where(data => data is Spectra).Select(data => (Spectra)data);
        if (!output.Exists) output.Create();
        if (spectras.Any())
            Parallel.ForEach(spectras, spectra => SaveSpectraAs(spectra, output, extension));
    }

    private void SaveSpectraAs(Spectra spectra, DirectoryInfo output, string extension) {
        var fullName = Path.Combine(output.FullName, $"{spectra.Name}{extension}");
        source.WriteData(spectra, fullName);
    }

    private Dictionary<DataSetNode, DirectoryInfo> LinkNodesAndOutputFolder(DataSetNode rootNode, DirectoryInfo outputRoot) {
        var track = new Dictionary<DataSetNode, DirectoryInfo> { [rootNode] = outputRoot };
        var queue = new Queue<DataSetNode>();
        queue.Enqueue(rootNode);
        while (queue.Count != 0) {
            var nodeInReference = queue.Dequeue();
            foreach (var nextNodeInReference in nodeInReference.Nodes) {
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

    public void DeleteSet() {
        if (ContextSet is null)
            throw new Exception("SelectedDataNode somehow is null");

        if (storage.ContainsSet(ContextSet.Name) && ContextSet != storage.DefaultDataSet)
            storage.RemoveSet(ContextSet.Name);
        else
            ContextSet.DisconnectFromParent();

        OnDataChanged?.Invoke();
    }

    public void DeleteData() {
        if (ContextData is null)
            throw new Exception("SelectedData somehow is null");

        ContextSet.Remove(ContextData);
        OnDataChanged?.Invoke();
    }

    #endregion

    #region SupportMethods

    public IEnumerable<TreeNode> GetRootTree() => workingDirectory.GetDirectoryTree();

    public IEnumerable<TreeNode> GetDataTree() {
        foreach (var pair in storage) {
            var node = new TreeNode { Text = pair.Key, Tag = pair.Value };
            ConnectDataSubnodes(node);
            yield return node;
        }
    }

    public IEnumerable<TreeNode> GetPlotTree() => plotContainer.GetPlotNodes();

    private string? SelectPathInExplorer() {
        using (FolderBrowserDialog dialog = new()) {
            dialog.SelectedPath = workingDirectory.FullName;
            DialogResult result = dialog.ShowDialog();
            return result == DialogResult.OK ? dialog.SelectedPath : null;
        }
    }

    private void ConnectDataSubnodes(TreeNode treeNode) {
        if (treeNode.Tag is not DataSetNode dataNode)
            throw new Exception(nameof(ConnectDataSubnodes));

        foreach (var child in dataNode.Nodes) {
            var subnode = new TreeNode {
                Text = child.Name,
                Tag = child,
            };
            ConnectDataSubnodes(subnode);
            treeNode.Nodes.Add(subnode);
        }

        foreach (var data in dataNode.Data) {
            var subnode = new TreeNode() {
                Text = data.Name,
                Tag = data,
            };
            treeNode.Nodes.Add(subnode);
        }
    }

    #endregion
}
