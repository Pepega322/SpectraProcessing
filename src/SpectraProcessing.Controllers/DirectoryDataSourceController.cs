using Microsoft.Extensions.Options;
using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Controllers.Settings;
using SpectraProcessing.Domain.InputOutput;
using SpectraProcessing.Domain.Storage;

namespace SpectraProcessing.Controllers;

public sealed class DirectoryDataSourceController<TData>(
    IOptions<DataReaderControllerSettings> settings,
    IDataReader<TData> reader
) : IDataSourceController<TData> where TData : class
{
    public event Action? OnChange;
    public DirectoryInfo Root { get; private set; } = new(settings.Value.StartFolderPath);

    public async Task<TData?> Read(string fullName)
    {
        try
        {
            return await Task.Run(() => reader.Get(fullName));
        }
        catch
        {
            return null;
        }
    }

    public async Task<DataSet<TData>> ReadFolderAsync()
    {
        var set = new DataSet<TData>(Root.Name);
        await Task.Run(
            () =>
            {
                Parallel.ForEach(
                    Root.GetFiles(),
                    file =>
                    {
                        var data = Read(file.FullName).Result;
                        if (data is not null)
                            set.AddThreadSafe(data);
                    });
            });
        return set;
    }

    public async Task<DataSet<TData>> ReadFolderFullDepthAsync()
    {
        var rootSet = new DataSet<TData>(Root.Name);
        await Task.Run(
            () =>
            {
                var queue = new Queue<(DataSet<TData> Node, DirectoryInfo Directory)>();
                queue.Enqueue((rootSet, Root));
                while (queue.Count > 0)
                {
                    var (node, dir) = queue.Dequeue();
                    Parallel.ForEach(
                        dir.GetFiles(),
                        (file) =>
                        {
                            var data = Read(file.FullName).Result;
                            if (data is not null)
                                node.AddThreadSafe(data);
                        });
                    foreach (var subdirectory in dir.GetDirectories())
                    {
                        var subnode = new DataSet<TData>(subdirectory.Name);
                        node.AddSubsetThreadSafe(subnode);
                        queue.Enqueue((subnode, subdirectory));
                    }
                }
            });

        return rootSet;
    }

    public bool ChangeFolder(string path)
    {
        var newDir = new DirectoryInfo(path);
        if (!newDir.Exists)
        {
            return false;
        }

        Root = new DirectoryInfo(path);
        OnChange?.Invoke();
        return true;
    }

    public bool StepOutFolder()
    {
        return Root.Parent is not null && ChangeFolder(Root.Parent.FullName);
    }

    public void RefreshView()
    {
        OnChange?.Invoke();
    }
}
