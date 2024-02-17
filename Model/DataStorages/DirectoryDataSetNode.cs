using Model.DataFormats;
using Model.DataSources;

namespace Model.DataStorages;
public class DirectoryDataSetNode : TreeDataSetNode {
    public DirectoryDataSetNode(string name)
        : base(name) { }

    public DirectoryDataSetNode(string name, DataReader reader, string path, bool addSubdirs = false, TreeDataSetNode? parent = null)
        : base(name, parent) {
        Parallel.ForEach(Directory.GetFiles(path), (f) => ReadData(reader, f));
        if (addSubdirs)
            Parallel.ForEach(Directory.GetDirectories(path), (d) => AddSubnodes(reader, d));
    }

    protected override void ReadData(DataReader reader, string pathForReader)
        => Add(reader.ReadData(pathForReader));

    protected override void AddSubnodes(DataReader reader, string pathForReader) {
        var name = Path.GetFileName(pathForReader);
        if (name is null)
            throw new DirectoryNotFoundException(pathForReader);
        var node = new DirectoryDataSetNode(name, reader, pathForReader, true, this);
        if (node.DataCount != 0)
            lock (subsets) subsets.Add(node);
    }

    public override bool Add(Data data) {
        if (data is not Spectra) return false;
        return AddToSet(data);
    }

    public override bool Remove(Data data) {
        if (data is not Spectra) return false;
        return RemoveFromSet(data);
    }
}
