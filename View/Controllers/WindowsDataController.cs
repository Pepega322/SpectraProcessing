using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;
using Model.Controllers;

namespace View.Controllers;
public class WindowsDataController : DataController, ITree {
    public WindowsDataController(DataWriter writer)
        : base(writer, new DirectoryDataTreeStorage()) { }

    public override async Task WriteSetAsAsync(DataSet set, string path, string extension, bool writeSubsets)
        => await Task.Run(() => WriteSetAs((DirectoryDataSetNode)set, path, extension, writeSubsets));

    public override async Task WriteDataAsAsync(Data data, string path, string extension)
        => await Task.Run(() => WriteDataAs(data, path, extension));

    private void WriteSetAs(DirectoryDataSetNode set, string path, string extension, bool writeSubsets,
        Dictionary<DirectoryDataSetNode, string>? track = null) {
        switch (writeSubsets) {
            case false:
                var data = set.Where(data => data is IWriteable);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (data.Any())
                    Parallel.ForEach(data, d => WriteDataAs(d, path, extension));
                break;
            case true:
                if (track == null)
                    track = LinkNodesAndOutputFolder(set, path);
                Parallel.ForEach(track.Keys, set => WriteSetAs(set, track[set], extension, true, track));
                break;

        }
    }

    private void WriteDataAs(Data data, string path, string extension) {
        if (data is IWriteable writeable) {
            var fullName = Path.Combine(path, $"{data.Name}{extension}");
            writer.WriteData(writeable, fullName);
        }
    }

    private void ConnectDataSubnodes(TreeNode treeNode) {
        if (treeNode.Tag is not DirectoryDataSetNode dataNode)
            throw new Exception(nameof(ConnectDataSubnodes));

        foreach (var child in dataNode.Subsets.OrderByDescending(child => child.Name)) {
            var subnode = new TreeNode {
                Text = child.Name,
                Tag = child,
            };
            ConnectDataSubnodes(subnode);
            treeNode.Nodes.Add(subnode);
        }

        foreach (var data in dataNode.OrderByDescending(data => data.Name)) {
            var subnode = new TreeNode() {
                Text = data.Name,
                Tag = data,
            };
            treeNode.Nodes.Add(subnode);
        }
    }

    private Dictionary<DirectoryDataSetNode, string> LinkNodesAndOutputFolder(DirectoryDataSetNode set, string path) {
        var track = new Dictionary<DirectoryDataSetNode, string> { [set] = path };
        var queue = new Queue<DirectoryDataSetNode>();
        queue.Enqueue(set);
        while (queue.Count != 0) {
            var nodeInReference = queue.Dequeue();
            foreach (var subset in nodeInReference.Subsets) {
                var nextNodeInReference = (DirectoryDataSetNode)subset;
                var pathInDestination = Path.Combine(track[nodeInReference], subset.Name);
                track[nextNodeInReference] = pathInDestination;
                queue.Enqueue(nextNodeInReference);
            }
        }
        return track;
    }

    public IEnumerable<TreeNode> GetTree() {
        foreach (var pair in storage) {
            var node = new TreeNode { Text = pair.Key, Tag = pair.Value };
            ConnectDataSubnodes(node);
            yield return node;
        }
    }
}
