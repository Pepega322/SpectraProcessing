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

    public async Task SubstractBaselineForSetAsync(DataSet set, bool includeSubsets)
        => await Task.Run(() => SubstractBaselineForSet(set, includeSubsets));

    public async Task SubstractBaselineForDataAsync(DataSet dataOwner, Data data)
        => await Task.Run(() => SubstractBaselineForData(dataOwner, data));

    private void SubstractBaselineForSet(DataSet set, bool includeSubsets) {
        switch (includeSubsets) {
            case false:
                Parallel.ForEach(set, d => SubstractBaselineForData(set, d));
                break;
            case true:
                var track = GetTrack((DirectoryDataSetNode)set, "-b");
                Parallel.ForEach(track, pair => SubstractBaselineForTrack(pair.Key, pair.Value));
                break;
        }
    }

    private void SubstractBaselineForTrack(DataSet source, DataSet destination) {
        foreach (var data in source) {
            if (data is not Spectra spectra) throw new Exception();
            var substraction = spectra.SubstractBaseLine();
            destination.Add(substraction);
        }
    }


    private void SubstractBaselineForData(DataSet dataOwner, Data data) {
        //if (data is not Spectra spectra || dataOwner is not TreeDataSetNode owner)
        //    throw new Exception();
        //var newSetName = $"{dataOwner.Name} -b";
        //DataSet subset;
        //if (owner.Parent == null) {
        //    if (!storage.ContainsSet(newSetName))
        //        storage.Add(newSetName, new DirectoryDataSetNode(newSetName));
        //    subset = storage[newSetName];
        //}
        //else {
        //    owner.Parent.AddSubset($"{dataOwner.Name} -b", out TreeDataSetNode s);
        //    subset = s;
        //}
        //var substracted = spectra.SubstractBaseLine();
        //subset.Add(substracted);
        throw new NotImplementedException();
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

    private Dictionary<TreeDataSetNode, TreeDataSetNode> GetTrack(TreeDataSetNode root, string addToName) {
        var track = new Dictionary<TreeDataSetNode, TreeDataSetNode>();
        TreeDataSetNode newNode;
        if (root.Parent == null) {
            newNode = new DirectoryDataSetNode($"{root.Name} {addToName}");
            storage.Add($"{root.Name} {addToName}", newNode);
        }
        else {
            newNode = new DirectoryDataSetNode($"{root.Name} {addToName}", (DirectoryDataSetNode)root.Parent);
            root.Parent.AddSubset(newNode);
        }
        track.Add(root, newNode);

        var queue = new Queue<TreeDataSetNode>();
        queue.Enqueue(root);
        while (queue.Count > 0) {
            var set = queue.Dequeue();
            foreach (var subset in set.Subsets.Where(s => s.DataCount > 0)) {
                var parent = track[set];
                var newName = $"{subset.Name} {addToName}";
                var newSet = new DirectoryDataSetNode(newName, (DirectoryDataSetNode)parent);
                parent.AddSubset(newSet);
                track.Add(subset, newSet);
                queue.Enqueue(subset);
            }
        }

        return track;
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
