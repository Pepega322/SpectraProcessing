using Model.DataSources;

namespace Model.DataStorages;
public class DirectoryDataSet : TreeSet {
    public DirectoryDataSet(string name)
        : base(name) { }

    public DirectoryDataSet(string name, DataReader reader, string path, bool addSubdirs = false, TreeSet? parent = null)
        : base(name, parent) {
        Parallel.ForEach(Directory.GetFiles(path), (f) => AddData(reader, f));
        if (addSubdirs)
            Parallel.ForEach(Directory.GetDirectories(path), (d) => AddSubnodes(reader, d));
    }

    protected override void AddData(DataReader reader, string pathForReader) 
        => Add(reader.ReadData(pathForReader));

    protected override void AddSubnodes(DataReader reader, string pathForReader) {
        var name = Path.GetFileName(pathForReader);
        if (name is null)
            throw new DirectoryNotFoundException(pathForReader);
        var node = new DirectoryDataSet(name, reader, pathForReader, true, this);
        if (node.DataCount != 0)
            lock (subsets) subsets.Add(node);
    }
}
