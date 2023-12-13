using Model.DataFormats.Spectras.Base;
using Model.DataSources.Base;
using Model.DataStorages.Base;

namespace Model.DataStorages;
public class DirDataSet : DataSetNode
{
    public DirDataSet(string name)
        : base(name) { }

    public DirDataSet(string name, DataSource source, string path, bool addSubdirs = false, DataSetNode? parent = null)
        : base(name, parent)
    {
        Parallel.ForEach(Directory.GetFiles(path), (f) => InitializeData(source, f));
        if (addSubdirs)
            Parallel.ForEach(Directory.GetDirectories(path), (d) => InitializeNodes(source, d));
    }

    protected override void InitializeData(DataSource source, string pathForSource)
    {
        var data = source.ReadData(pathForSource);
        if (data is Spectra) AddData(data);
    }

    protected override void InitializeNodes(DataSource source, string pathForSource)
    {
        var name = Path.GetFileName(pathForSource);
        if (name is null)
            throw new DirectoryNotFoundException(pathForSource);
        var node = new DirDataSet(name, source, pathForSource, true, this);
        if (node.DataCount != 0)
            lock (_nodes) _nodes.Add(node);
    }
}
