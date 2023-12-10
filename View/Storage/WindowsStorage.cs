using System.Collections;
using Model.SupportedDataFormats.Undefined;
using Model.SupportedDataSources.Base;

namespace View.Storage;
public class WindowsStorage : IEnumerable<KeyValuePair<string, WindowsDataNode>>
{
    public const string TempDataSetKey = "TempData";

    public string Name { get; protected set; }
    private readonly DataSource _source;
    private readonly Dictionary<string, WindowsDataNode> _storage;

    public WindowsStorage(string name, DataSource source)
    {
        Name = name;
        _source = source;
        _storage = [];
        _storage.Add(TempDataSetKey, new WindowsDataNode(_source, TempDataSetKey));
    }

    public void AddDirectoryAsOneDataSet(string dataSetKey, DirectoryInfo root)
    {
        if (_storage.ContainsKey(dataSetKey))
            throw new ArgumentException($"{dataSetKey} already exist");
        _storage.Add(dataSetKey, new WindowsDataNode(_source, root));
    }

    public void AddFileToTempDataSet(FileInfo file)
    {
        var data = _source.ReadFile(file.FullName);
        if (data is not Undefined)
            lock (_storage) _storage[TempDataSetKey].AppendData(data);
    }

    public IEnumerator<KeyValuePair<string, WindowsDataNode>> GetEnumerator()
    {
        foreach (var pair in _storage)
            yield return pair;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
