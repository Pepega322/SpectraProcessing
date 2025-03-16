using SpectraProcessing.Controllers.Interfaces;
using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Domain.InputOutput;
using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Controllers;

public sealed class DirectoryDataWriterController(IDataWriter writer) : IDataWriterController
{
    public Task DataWriteAs<TData>(TData data, string path)
        where TData : IWriteableData
    {
        return writer.WriteData(data, path);
    }

    public Task SetOnlyWriteAs<TData>(DataSet<TData> set, string path, string extension)
        where TData : IWriteableData
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return Task.WhenAll(set.Data.Select(data => DataWriteAs(data, Path.Combine(path, $"{data.Name}{extension}"))));
    }

    public async Task SetFullDepthWriteAs<TData>(DataSet<TData> root, string path, string extension)
        where TData : IWriteableData
    {
        var track = LinkSetAndOutputFolder(root, path);

        await Task.WhenAll(track.Keys.Select(set => SetOnlyWriteAs(set, track[set], extension)));
    }

    private static Dictionary<DataSet<TData>, string> LinkSetAndOutputFolder<TData>(DataSet<TData> set, string path)
        where TData : IWriteableData
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
