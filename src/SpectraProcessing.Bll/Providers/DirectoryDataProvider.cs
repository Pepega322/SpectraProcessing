using Microsoft.Extensions.Logging;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Dal.Repositories.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Bll.Providers;

internal sealed class DirectoryDataProvider<TData>(
    IDataRepository<TData> dataRepository,
    ILogger<DirectoryDataProvider<TData>> logger
) : IDataProvider<TData>
    where TData : class, IWriteableData
{
    public async Task<DataSet<TData>> ReadFolderAsync(string fullName)
    {
        var folder = new DirectoryInfo(fullName);

        var set = new DataSet<TData>(folder.Name);

        foreach (var file in folder.GetFiles())
        {
            try
            {
                var data = await dataRepository.ReadData(file.FullName);

                set.AddThreadSafe(data);
            }
            catch (Exception e)
            {
                logger.LogError(
                    "Error while reading files: Error: {Error}, Message: {Message}",
                    e,
                    e.Message);
            }
        }

        return set;
    }

    public async Task<DataSet<TData>> ReadFolderFullDepthAsync(string fullName)
    {
        var folder = new DirectoryInfo(fullName);

        var rootSet = new DataSet<TData>(folder.Name);

        var queue = new Queue<(DataSet<TData> Node, DirectoryInfo Directory)>();

        queue.Enqueue((rootSet, folder));

        while (queue.Count > 0)
        {
            var (node, dir) = queue.Dequeue();

            await ReadFolder(dir, node);

            foreach (var subdirectory in dir.GetDirectories())
            {
                var subnode = new DataSet<TData>(subdirectory.Name);
                node.AddSubsetThreadSafe(subnode);
                queue.Enqueue((subnode, subdirectory));
            }
        }

        await Task.Run(() => { });

        return rootSet;
    }

    public Task DataWriteAs(TData data, string path)
    {
        return dataRepository.WriteData(data, path);
    }

    public Task SetOnlyWriteAs(DataSet<TData> set, string path, string extension)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return Task.WhenAll(set.Data.Select(data => DataWriteAs(data, Path.Combine(path, $"{data.Name}{extension}"))));
    }

    public async Task SetFullDepthWriteAs(DataSet<TData> root, string path, string extension)
    {
        var track = LinkSetAndOutputFolder(root, path);

        await Task.WhenAll(track.Keys.Select(set => SetOnlyWriteAs(set, track[set], extension)));
    }

    private async Task ReadFolder(DirectoryInfo dir, DataSet<TData> node)
    {
        foreach (var file in dir.GetFiles())
        {
            try
            {
                var data = await dataRepository.ReadData(file.FullName);

                node.AddThreadSafe(data);
            }
            catch (Exception e)
            {
                logger.LogError(
                    "Error while reading files: Error: {Error}, Message: {Message}",
                    e,
                    e.Message);
            }
        }
    }

    private static Dictionary<DataSet<TData>, string> LinkSetAndOutputFolder(DataSet<TData> set, string path)
    {
        var track = new Dictionary<DataSet<TData>, string> { [set] = path };

        var queue = new Queue<DataSet<TData>>();

        queue.Enqueue(set);

        while (queue.Count != 0)
        {
            var nodeInReference = queue.Dequeue();

            foreach (var subset in nodeInReference.Subsets)
            {
                var pathInDestination = Path.Combine(track[nodeInReference], subset.Name);
                track[subset] = pathInDestination;
                queue.Enqueue(subset);
            }
        }

        return track;
    }
}
