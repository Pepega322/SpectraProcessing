using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;
using Model.Controllers;

namespace View.Controllers;
public class WindowsDataController : DataController, ITree {
    public WindowsDataController(DataWriter writer)
        : base(writer, new DirectoryDataTreeStorage("Single Data")) { }

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

    public async Task SubstractBaselineForSetAsync(DataSet set, bool includeSubsets)
        => await Task.Run(() => SubstractBaselineForSet(set, includeSubsets));

    public async Task SubstractBaselineForDataAsync(Data data)
        => await Task.Run(() => SubstractBaseline(data, storage.DefaultSet));

    private void SubstractBaselineForSet(DataSet set, bool includeSubsets) {
        switch (includeSubsets) {
            case false:
                var destination = new DirectoryDataSetNode($"{set.Name} -b (only)");
                storage.Add(destination.Name, destination);
                Parallel.ForEach(set, d => SubstractBaseline(d, destination));
                break;
            case true:
                var root = ((DirectoryDataSetNode)set).CopyBranchStructure($"{set.Name} -b (all)", out Dictionary<TreeDataSetNode, TreeDataSetNode> refToCopy);
                storage.Add(root.Name, root);
                foreach (var reference in refToCopy.Keys) {
                    var copy = refToCopy[reference];
                    Parallel.ForEach(reference, d => SubstractBaseline(d, copy));
                }
                break;
        }
    }

    private void SubstractBaseline(Data data, DataSet destination) {
        if (data is not Spectra spectra) throw new Exception();
        var substracted = spectra.SubstractBaseLine();
        lock (destination) destination.Add(substracted);
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

    public IEnumerable<TreeNode> GetTree() {
        foreach (var pair in storage) {
            var node = new TreeNode { Text = pair.Key, Tag = pair.Value };
            ConnectDataSubnodes(node);
            yield return node;
        }
    }
}
