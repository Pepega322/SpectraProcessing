using Model.SupportedDataFormats.SupportedSpectraFormats.Base;
using Model.SupportedDataSources.Windows;
using View.Storage;

namespace View.Controllers.Windows;
public class WindowsController
{
    private readonly WindowsFileSystem _source;
    private readonly WindowsStorage _storage;
    private DirectoryInfo _root;

    public event Action? OnDataAdd;
    public event Action? OnRootChange;

    public WindowsController(string storageName)
    {
        _root = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        _source = new();
        _storage = new(storageName, _source);
    }

    public async void ReadRootAsSeriesAsync()
    {
        await Task.Run(() => _storage.AddDirectoryAsOneDataSet(_root.Name, _root));
        OnDataAdd?.Invoke();
    }

    private void ChangeRoot(DirectoryInfo newRoot)
    {
        _root = newRoot;
        OnRootChange?.Invoke();
    }

    public void RootSelect()
    {
        using (FolderBrowserDialog dialog = new())
        {
            dialog.SelectedPath = _root.FullName;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
                ChangeRoot(new DirectoryInfo(dialog.SelectedPath));
        }
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
            OnDataAdd?.Invoke();
        }
    }

    public (double[] sX, double[] sY) GetSpectraPoints(Spectra spectra)
    {
        var points = spectra.GetPoints();
        var sX = points.Select(p => (double)p.X).ToArray();
        var sY = points.Select(p => (double)p.Y).ToArray();
        return (sX, sY);
    }

    #region GetTreeNodes

    public IEnumerable<TreeNode> GetRootNodes()
    {
        var nodes = new List<TreeNode>();
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
            ConnectSubnodes(node);
            yield return node;
        }
    }

    private void ConnectSubnodes(TreeNode treeNode)
    {
        var dataNode = treeNode.Tag as WindowsDataNode;
        foreach (var child in dataNode.Childrens)
        {
            var subnode = new TreeNode
            {
                Text = child.Name,
                Tag = child,
            };
            ConnectSubnodes(subnode);
            treeNode.Nodes.Add(subnode);
        }

        foreach (var data in dataNode.Data)
        {
            var subnode = new TreeNode
            {
                Text = data.Name,
                Tag = data,
            };
            treeNode.Nodes.Add(subnode);
        }
    }

    #endregion
}
