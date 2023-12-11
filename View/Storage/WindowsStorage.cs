using System.Collections;
using Model.SupportedDataFormats.Unsupported;
using Model.SupportedDataSources.Base;

namespace View.Storage;
public class WindowsStorage : IEnumerable<KeyValuePair<string, DataNode>>
{
    public const string DefaultDataSetKey = "Default";

    public string Name { get; protected set; }
    public DataNode DefaultDataNode => _storage[DefaultDataSetKey];

    private readonly DataSource _source;
    private readonly Dictionary<string, DataNode> _storage;

    public WindowsStorage(string name, DataSource source)
    {
        Name = name;
        _source = source;
        _storage = [];
        _storage.Add(DefaultDataSetKey, new DataNode(_source, DefaultDataSetKey));
    }

    public void ReadAllDirectory(string dataSetKey, DirectoryInfo root)
    {
        if (_storage.ContainsKey(dataSetKey))
            throw new ArgumentException($"Series with name \n{dataSetKey}\n already exist");
        _storage.Add(dataSetKey, new DataNode(_source, dataSetKey, root, true));
    }

    public void ReadThisDirectory(string dataSetKey, DirectoryInfo directory)
    {
        if (_storage.ContainsKey(dataSetKey))
            throw new ArgumentException($"Series with name \n{dataSetKey}\n already exist");
        _storage.Add(dataSetKey, new DataNode(_source, dataSetKey, directory, false));
    }

    public bool Contains(string dataSetKey) => _storage.ContainsKey(dataSetKey);

    public bool Remove(string dataSetKey) => _storage.Remove(dataSetKey);

    public void AddFileToTempDataSet(FileInfo file)
    {
        var data = _source.ReadFile(file.FullName);
        if (data is not Undefined and not Empty)
            lock (_storage) _storage[DefaultDataSetKey].AppendData(data);
    }

    public IEnumerator<KeyValuePair<string, DataNode>> GetEnumerator()
    {
        foreach (var pair in _storage)
            yield return pair;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
