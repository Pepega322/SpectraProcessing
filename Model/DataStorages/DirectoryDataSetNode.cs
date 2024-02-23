using Model.DataFormats;
using Model.DataSources;

namespace Model.DataStorages;
public class DirectoryDataSetNode : TreeDataSetNode {
    public DirectoryDataSetNode(string name, DirectoryDataSetNode? parent = null)
        : base(name, parent) { }

    public static DirectoryDataSetNode ReadDirectory(string setName, DataReader reader, string pathToRoot, bool addSubdirs = false) {
        var rootSet = new DirectoryDataSetNode(setName, null);
        var rootDir = new DirectoryInfo(pathToRoot);
        if (!rootDir.Exists)
            throw new DirectoryNotFoundException(pathToRoot);
        switch (addSubdirs) {
            case false:
                Parallel.ForEach(rootDir.GetFiles(), (file) => rootSet.Add(reader.ReadData(file.FullName)));
                break;
            case true:
                var queue = new Queue<(DirectoryDataSetNode Node, DirectoryInfo Directory)>();
                queue.Enqueue((rootSet, rootDir));
                while (queue.Count > 0) {
                    var info = queue.Dequeue();
                    var node = info.Node;
                    var dir = info.Directory;
                    Parallel.ForEach(dir.GetFiles(), file => node.Add(reader.ReadData(file.FullName)));
                    foreach (var subdir in dir.GetDirectories()) {
                        var subnode = new DirectoryDataSetNode(subdir.Name, node);
                        node.AddSubset(subnode);
                        queue.Enqueue((subnode, subdir));
                    }
                }
                break;
        }
        return rootSet;
    }

    public override bool Add(Data data) {
        if (data is not Spectra) return false;
        return AddToSet(data);
    }

    public override bool Remove(Data data) {
        if (data is not Spectra) return false;
        return RemoveFromSet(data);
    }

    public override TreeDataSetNode CopyBranchStructure(string rootName, out Dictionary<TreeDataSetNode, TreeDataSetNode> refToCopy) {
        var result = new DirectoryDataSetNode(rootName);
        refToCopy = new Dictionary<TreeDataSetNode, TreeDataSetNode>();
        refToCopy.Add(this, result);
        var queue = new Queue<TreeDataSetNode>();
        queue.Enqueue(this);

        while (queue.Count > 0) {
            var reference = queue.Dequeue();
            var copy = (DirectoryDataSetNode)refToCopy[reference];
            foreach (DirectoryDataSetNode subsetInRef in reference.Subsets) {
                var subsetInCopy = new DirectoryDataSetNode(subsetInRef.Name, copy);
                copy.AddSubset(subsetInCopy);
                refToCopy.Add(subsetInRef, subsetInCopy);
                queue.Enqueue(subsetInRef);
            }
        }
        return result;
    }
}
