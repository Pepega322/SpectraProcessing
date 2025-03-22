using Microsoft.Extensions.Logging;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Domain.InputOutput;
using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Bll.Controllers;

public sealed class DirectoryDataSourceController<TData>(
    IDataReader<TData> reader,
    ILogger<DirectoryDataSourceController<TData>> logger
) : IDataSourceController<TData>
    where TData : class
{
    public async Task<DataSet<TData>> ReadFolderAsync(string fullName)
    {
        var folder = new DirectoryInfo(fullName);

        var set = new DataSet<TData>(folder.Name);

        foreach (var file in folder.GetFiles())
        {
            try
            {
                var data = await reader.ReadData(file.FullName);

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

    private async Task ReadFolder(DirectoryInfo dir, DataSet<TData> node)
    {
        foreach (var file in dir.GetFiles())
        {
            try
            {
                var data = await reader.ReadData(file.FullName);

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
}
