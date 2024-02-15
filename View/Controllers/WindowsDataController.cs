using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;
using System.Xml.Linq;

namespace View.Controllers;
public class WindowsDataController : DataController {
    public WindowsDataController(DataWriter writer)
        : base(writer, new DirectoryBasedStorage()) { }
   
    public override async Task WriteSetAsAsync(string path, string extension, bool writeSubsets)
        => await Task.Run(() => WriteSetAs(ContextSet, path, extension, writeSubsets));

    public override async Task WriteDataAsAsync(string path, string extension)
        => await Task.Run(() => WriteDataAs(ContextData, path, extension));

    private void WriteSetAs(DataSetNode set, string path, string extension, bool writeSubsets, 
        Dictionary<DataSetNode, string>? track = null) {
        switch (writeSubsets) {
            case false:
                var data = set.Data.Where(data => data is IWriteable);
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
        if (data is IWriteable) {
            var fullName = Path.Combine(path, $"{data.Name}{extension}");
            writer.WriteData((IWriteable)ContextData, fullName);
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

    private Dictionary<DataSetNode, string> LinkNodesAndOutputFolder(DataSetNode set, string path) {
        var track = new Dictionary<DataSetNode, string> { [set] = path };
        var queue = new Queue<DataSetNode>();
        queue.Enqueue(set);
        while (queue.Count != 0) {
            var nodeInReference = queue.Dequeue();
            foreach (var nextNodeInReference in nodeInReference.Nodes) {
                var pathInDestination = Path.Combine(track[nodeInReference], nextNodeInReference.Name);
                track[nextNodeInReference] = pathInDestination;
                queue.Enqueue(nextNodeInReference);
            }
        }
        return track;
    }

    public override IEnumerable<TreeNode> GetTree() {
        foreach (var pair in storage) {
            var node = new TreeNode { Text = pair.Key, Tag = pair.Value };
            ConnectDataSubnodes(node);
            yield return node;
        }
    }
}
