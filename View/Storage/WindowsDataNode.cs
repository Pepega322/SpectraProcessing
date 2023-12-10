using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Undefined;
using Model.SupportedDataSources.Base;

namespace View.Storage;
public class WindowsDataNode
{
    public readonly string Name;
    private readonly DataSource _source;
    private readonly WindowsDataNode? _parent;
    private readonly DirectoryInfo? _directory;
    private readonly HashSet<Data> _dataSet;
    private readonly HashSet<WindowsDataNode> _childrens;
    public WindowsDataNode? Parent => _parent;
    public DirectoryInfo? Directory => _directory;
    public IEnumerable<Data> Data => _dataSet;
    public IEnumerable<WindowsDataNode> Childrens => _childrens;

    public WindowsDataNode(DataSource source, DirectoryInfo directory, WindowsDataNode? parent = null)
    {
        Name = directory.Name;
        _source = source;
        _parent = parent;
        _directory = directory;
        _dataSet = [];
        Parallel.ForEach(directory.GetFiles(), AddData);
        _childrens = [];
        Parallel.ForEach(directory.GetDirectories(), AddNode);
    }

    public WindowsDataNode(DataSource source, string name)
    {
        Name = name;
        _source = source;
        _dataSet = [];
        _childrens = [];
    }

    public void AppendData(Data data)
    {
        if (_dataSet.All(d => d.Name != data.Name))
            _dataSet.Add(data);
    }

    private void AddData(FileInfo file)
    {
        var data = _source.ReadFile(file.FullName);
        if (data is not Undefined)
        lock (_dataSet) _dataSet.Add(data);
    }

    private void AddNode(DirectoryInfo directory)
    {
        var node = new WindowsDataNode(_source, directory, this);
        lock (_childrens) _childrens.Add(node);
    }
}
