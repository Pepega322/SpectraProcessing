using Model.Connectors.Base;
using Model.SupportedDataSources.Windows;
using Model.SupportedStorages;

namespace Model.Connectors;
internal class WindowsConnector : Connector
{
    private readonly WindowsFileSystem _source;
    private readonly DataStorage _storage;

    public WindowsConnector(WindowsFileSystem source, DataStorage storage)
    {
        _source = source;
        _storage = storage;
    }

    public void AddSeries(string seriesID, string seriesName, DirectoryInfo root)
    {
        if (_storage.ContainsID(seriesID))
            throw new ArgumentException($"{seriesID} already contains in storage");
        var series = ReadDirectory(seriesName, root);
        _storage.Add(seriesID, series);
    }

    public void AddSeries(string seriesID, DirectoryInfo root)
        => AddSeries(seriesID, root.Name, root);

    public async Task AddSeriesAsync(string seriesID, string seriesName, DirectoryInfo root)
        => await Task.Run(() => AddSeries(seriesID, seriesName, root));

    public async Task AddSeriesAsync(string seriesID, DirectoryInfo root)
        => await Task.Run(() => AddSeries(seriesID, root));

    private DataSeries ReadDirectory(string name, DirectoryInfo root)
    {
        var series = new DataSeries(name);
        var stack = new Stack<DirectoryInfo>();
        stack.Push(root);
        while (stack.Count != 0)
        {
            var dir = stack.Pop();
            var files = _source.ReadFilesAtDirectory(dir);
            foreach (var data in files)
            {
                var id = Path.Combine(dir.FullName, data.Name);
                series.Add(id, data);
            }
            foreach (var d in dir.GetDirectories())
                stack.Push(d);
        }
        return series;
    }

    private DataSeries ReadDirectory(DirectoryInfo root)
        => ReadDirectory(root.Name, root);

    private async Task<DataSeries> ReadDirectoryAsync(string name, DirectoryInfo root)
        => await Task.Run(() => ReadDirectory(name, root));

    private async Task<DataSeries> ReadDirectoryAsync(DirectoryInfo root)
        => await Task.Run(() => ReadDirectory(root));
}
