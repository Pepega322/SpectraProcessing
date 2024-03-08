using DataSource.FileSource;
using Domain;
using Domain.DataSource;

namespace Controllers;

public sealed class DirectoryDataWriterController<TData> where TData : Data, IWriteable {
    private readonly FileWriter writer = new();
    public void DataWriteAs(TData data, string path) {
        writer.WriteData(data, path);
    }

    public void SetOnlyWriteAs(DataSet<TData> set, string path, string extension) {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        Parallel.ForEach(set, d => DataWriteAs(d, Path.Combine(path, $"{d.Name}{extension}")));
    }

    public void SetFullDepthWriteAs(DataSet<TData> root, string path, string extension) {
        var track = DirectoryDataWriterController<TData>.LinkSetAndOutputFolder(root, path);
        Parallel.ForEach(track.Keys, set => SetOnlyWriteAs(set, track[set], extension));
    }

    private static Dictionary<DataSet<TData>, string> LinkSetAndOutputFolder(DataSet<TData> set, string path) {
        var track = new Dictionary<DataSet<TData>, string> { [set] = path };
        var queue = new Queue<DataSet<TData>>();
        queue.Enqueue(set);
        while (queue.Count != 0) {
            var nodeInReference = queue.Dequeue();
            foreach (var subset in nodeInReference.Subsets) {
                var nextNodeInReference = subset;
                var pathInDestination = Path.Combine(track[nodeInReference], subset.Name);
                track[nextNodeInReference] = pathInDestination;
                queue.Enqueue(nextNodeInReference);
            }
        }
        return track;
    }
}