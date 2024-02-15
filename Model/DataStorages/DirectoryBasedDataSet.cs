using Model.DataFormats;
using Model.DataSources;

namespace Model.DataStorages;
public class DirDataSet : DataSetNode {
    public DirDataSet(string name)
        : base(name) { }

    public DirDataSet(string name, DataReader reader, string path, bool addSubdirs = false, DataSetNode? parent = null)
        : base(name, parent) {
        Parallel.ForEach(Directory.GetFiles(path), (f) => InitializeData(reader, f));
        if (addSubdirs)
            Parallel.ForEach(Directory.GetDirectories(path), (d) => InitializeNodes(reader, d));
    }

    protected override void InitializeData(DataReader reader, string pathForReader) {
        var data = reader.ReadData(pathForReader);
        if (data is Spectra) Add(data);
    }

    protected override void InitializeNodes(DataReader reader, string pathForReader) {
        var name = Path.GetFileName(pathForReader);
        if (name is null)
            throw new DirectoryNotFoundException(pathForReader);
        var node = new DirDataSet(name, reader, pathForReader, true, this);
        if (node.DataCount != 0)
            lock (nodes) nodes.Add(node);
    }
}
