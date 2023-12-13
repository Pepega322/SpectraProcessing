using System.Data;
using Model.DataFormats.Base;
using Model.DataFormats.Spectras.Base;
using Model.DataSources.Base;
using Model.DataSources.Windows;
using Model.DataStorages;
using Model.DataStorages.Base;

namespace Model.Controllers.Windows;
public class WindowsController
{
    private readonly DataSource _source;
    private readonly DataStorage _storage;

    private DirectoryInfo _selectedRoot;
    public DataSetNode SelectedSet { get; set; }
    public Data? SelectedData { get; set; }

    public event Action? OnDataChange;
    public event Action? OnRootChange;

    public WindowsController(string storageName)
    {
        //_selectedRoot = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        _selectedRoot = new DirectoryInfo("E:\\ForProgramTest");
        _source = new WindowsFileSystem();
        _storage = new DirectoryBasedStorage(_source, storageName);
        SelectedSet = _storage.DefaultDataSet;
    }

    #region DataReadMethods

    public async void RootReadAllAsync()
    {
        var setKey = $"{_selectedRoot.Name} (all)";
        var rootSet = await Task.Run(() => new DirDataSet(_selectedRoot.Name, _source, _selectedRoot.FullName, true));
        _storage.AddDataSet(setKey, rootSet);
        OnDataChange?.Invoke();
    }

    public async void RootReadThisAsync()
    {
        var setKey = $"{_selectedRoot.Name} (only this)";
        var rootSet = await Task.Run(() => new DirDataSet(_selectedRoot.Name, _source, _selectedRoot.FullName, false));
        _storage.AddDataSet(setKey, rootSet);
        OnDataChange?.Invoke();
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
        if (_selectedRoot.Parent != null)
            ChangeRoot(_selectedRoot.Parent);
    }

    public void RootStepInDoubleClick(TreeNodeMouseClickEventArgs args)

    {
        if (args.Node.Tag is DirectoryInfo newRoot)
        {
            ChangeRoot(newRoot);
            return;
        }

        if (args.Node.Tag is FileInfo file)
        {
            var data = _source.ReadData(file.FullName);
            _storage.AddDataToDefaultSet(data);
            OnDataChange?.Invoke();
        }
    }

    private void ChangeRoot(DirectoryInfo newRoot)
    {
        _selectedRoot = newRoot;
        OnRootChange?.Invoke();
    }

    #endregion

    #region SupportMethods

    private string? SelectPathInExplorer()
    {
        using (FolderBrowserDialog dialog = new())
        {
            dialog.SelectedPath = _selectedRoot.FullName;
            DialogResult result = dialog.ShowDialog();
            return result == DialogResult.OK ? dialog.SelectedPath : null;
        }
    }

    #endregion

    #region GetTreeNodesMethods

    public IEnumerable<TreeNode> GetRootNodes()
    {
        foreach (var dir in _selectedRoot.GetDirectories())
            yield return new TreeNode { Text = dir.Name, Tag = dir, ImageIndex = 0 };
        foreach (var file in _selectedRoot.GetFiles())
            yield return new TreeNode { Text = file.Name, Tag = file, ImageIndex = 1, };
    }

    public IEnumerable<TreeNode> GetDataNodes()
    {
        foreach (var pair in _storage)
        {
            var node = new TreeNode { Text = pair.Key, Tag = pair.Value };
            ConnectDataSubnodes(node);
            yield return node;
        }
    }

    private static void ConnectDataSubnodes(TreeNode treeNode)
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

    #region ConvertingMethods

    public async void SaveAllSeriesAsESPAsync()
    {
        var path = SelectPathInExplorer();
        if (path is null) return;
        var output = new DirectoryInfo(Path.Combine(path, $"{SelectedSet.Name} (converted all)"));
        await Task.Run(() => SaveAllSpectrasAs(SelectedSet, output, ".esp"));
    }

    public async void SaveThisSeriesAsESPAsync()
    {
        var path = SelectPathInExplorer();
        if (path is null) return;
        var output = new DirectoryInfo(Path.Combine(path, $"{SelectedSet.Name} (converted only this)"));
        await Task.Run(() => SaveThisSpectrasAs(SelectedSet, output, ".esp"));
    }

    public async void SaveAsESPAsync()
    {
        if (SelectedData is not Spectra spectra) return;
        var path = SelectPathInExplorer();
        if (path is null) return;
        await Task.Run(() => SaveSpectraAs(spectra, new DirectoryInfo(path), ".esp"));
    }

    private void SaveAllSpectrasAs(DataSetNode root, DirectoryInfo outputRoot, string extension)
    {
        var track = LinkNodesAndOutputFolder(root, outputRoot);
        foreach (var node in track.Keys)
            Task.Run(() => SaveThisSpectrasAs(node, track[node], extension));
    }

    private void SaveThisSpectrasAs(DataSetNode node, DirectoryInfo output, string extension)
    {
        var spectras = node.Data.Where(data => data is Spectra);
        if (!output.Exists) output.Create();
        if (!spectras.Any()) return;
        foreach (var spectra in spectras)
            Task.Run(() => SaveSpectraAs((Spectra)spectra, output, extension));
    }

    private void SaveSpectraAs(Spectra spectra, DirectoryInfo output, string extension)
    {
        var fullName = Path.Combine(output.FullName, $"{spectra.Name}{extension}");
        Task.Run(() => _source.WriteData(spectra, fullName));
    }

    private static Dictionary<DataSetNode, DirectoryInfo> LinkNodesAndOutputFolder(DataSetNode rootNode, DirectoryInfo outputRoot)
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

    public void DeleteNode()
    {
        if (SelectedSet is null)
            throw new Exception("SelectedDataNode somehow is null");

        if (!_storage.ContainsSet(SelectedSet.Name) || SelectedSet == _storage.DefaultDataSet)
            SelectedSet.DisconnectFromParent();
        else
            _storage.RemoveSet(SelectedSet.Name);

        OnDataChange?.Invoke();
    }

    public void DeleteData()
    {
        if (SelectedData is null)
            throw new Exception("SelectedData somehow is null");

        SelectedSet.RemoveData(SelectedData);
        OnDataChange?.Invoke();
    }

    #endregion

    #region PlotMethodsMethods

    public void AddToPlotTemp()
    {

    }

    #endregion

}
