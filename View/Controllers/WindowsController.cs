using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Interfaces;
using Model.SupportedDataFormats.SupportedSpectraFormats.Base;
using Model.SupportedDataSources.Windows;
using View.Storage;

namespace View.Controllers.Windows;
public class WindowsController
{
    private readonly WindowsFileSystem _source;
    private readonly WindowsStorage _storage;
    private DirectoryInfo _root;
    public DataNode SelectedNode { get; set; }
    public Data? SelectedData { get; set; }

    public event Action? OnDataChange;
    public event Action? OnRootChange;

    public WindowsController(string storageName)
    {
        _root = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        _source = new();
        _storage = new(storageName, _source);
        SelectedNode = _storage.DefaultDataNode;
    }

    #region DataReadMethods

    public async void ReadAllRootAsync()
    {
        await Task.Run(() => _storage.ReadAllDirectory($"{_root.Name} (all)", _root));
        OnDataChange?.Invoke();
    }

    public async void ReadThisRootAsync()
    {
        await Task.Run(() => _storage.ReadThisDirectory($"{_root.Name} (only this)", _root));
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
        if (_root.Parent != null)
            ChangeRoot(_root.Parent);
    }

    public void RootSelectDoubleClick(TreeNodeMouseClickEventArgs args)

    {
        if (args.Node.Tag is DirectoryInfo newRoot)
        {
            ChangeRoot(newRoot);
            return;
        }

        if (args.Node.Tag is FileInfo file)
        {
            _storage.AddFileToTempDataSet(file);
            OnDataChange?.Invoke();
        }
    }

    private void ChangeRoot(DirectoryInfo newRoot)
    {
        _root = newRoot;
        OnRootChange?.Invoke();
    }

    #endregion

    #region SupportMethods

    private string? SelectPathInExplorer()
    {
        using (FolderBrowserDialog dialog = new())
        {
            dialog.SelectedPath = _root.FullName;
            DialogResult result = dialog.ShowDialog();
            return result == DialogResult.OK ? dialog.SelectedPath : null;
        }
    }

    #endregion

    #region GetTreeNodesMethods

    public IEnumerable<TreeNode> GetRootNodes()
    {
        foreach (var dir in _root.GetDirectories())
            yield return new TreeNode { Text = dir.Name, Tag = dir, ImageIndex = 0 };
        foreach (var file in _root.GetFiles())
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
        var dataNode = treeNode.Tag as DataNode;
        //if (dataNode.Childrens != null)
        foreach (var child in dataNode.Childrens)
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

    public async void SaveAllSeriesAsESP()
    {
        var path = SelectPathInExplorer();
        if (path is null) return;
        var output = new DirectoryInfo(Path.Combine(path, $"{SelectedNode.Name} (converted all)"));
        await Task.Run(() => SaveAllSpectrasAs(SelectedNode, output, ".esp"));
    }

    public async void SaveThisSeriesAsESP()
    {
        var path = SelectPathInExplorer();
        if (path is null) return;
        var output = new DirectoryInfo(Path.Combine(path, $"{SelectedNode.Name} (converted only this)"));
        await Task.Run(() => SaveThisSpectrasAs(SelectedNode, output, ".esp"));
    }

    public async void SaveAsESP()
    {
        if (SelectedData is not Spectra spectra) return;
        var path = SelectPathInExplorer();
        if (path is null) return;
        await Task.Run(() => SaveSpectraAs(spectra, new DirectoryInfo(path), ".esp"));
    }

    private void SaveAllSpectrasAs(DataNode root, DirectoryInfo outputRoot, string extension)
    {
        var track = LinkNodesAndOutputFolder(root, outputRoot);
        foreach (var node in track.Keys)
            Task.Run(() => SaveThisSpectrasAs(node, track[node], extension));
    }

    private void SaveThisSpectrasAs(DataNode node, DirectoryInfo output, string extension)
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
        Task.Run(() => _source.WriteFile(spectra, fullName));
    }

    private static Dictionary<DataNode, DirectoryInfo> LinkNodesAndOutputFolder(DataNode rootNode, DirectoryInfo outputRoot)
    {
        var track = new Dictionary<DataNode, DirectoryInfo>
        {
            [rootNode] = outputRoot
        };
        var queue = new Queue<DataNode>();
        queue.Enqueue(rootNode);
        while (queue.Count != 0)
        {
            var nodeInReference = queue.Dequeue();
            foreach (var nextNodeInReference in nodeInReference.Childrens)
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
        if (SelectedNode is null)
            throw new Exception("SelectedDataNode somehow is null");

        if (!_storage.Contains(SelectedNode.Name) || SelectedNode == _storage.DefaultDataNode)
            SelectedNode.DisconnectFromParent();
        else
            _storage.Remove(SelectedNode.Name);

        OnDataChange?.Invoke();
    }

    public void DeleteData()
    {
        if (SelectedData is null)
            throw new Exception("SelectedData somehow is null");

        SelectedNode.RemoveData(SelectedData);
        OnDataChange?.Invoke();
    }

    #endregion

    #region PlotMethodsMethods

    public static (double[] sX, double[] sY) GetSpectraPoints(Spectra spectra)
    {
        var points = spectra.GetPoints();
        var sX = points.Select(p => (double)p.X).ToArray();
        var sY = points.Select(p => (double)p.Y).ToArray();
        return (sX, sY);
    }

    #endregion

}
