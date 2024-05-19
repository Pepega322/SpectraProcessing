using Controllers.Interfaces;
using Domain.InputOutput;
using Domain.Storage;

namespace Controllers;

public sealed class DirectoryDataWriterController(IDataWriter writer) : IDataWriterController
{
	public async Task DataWriteAs<TData>(TData data, string path)
		where TData : IWriteableData
	{
		await Task.Run(() => writer.WriteData(data, path));
	}

	public async Task SetOnlyWriteAs<TData>(DataSet<TData> set, string path, string extension)
		where TData : IWriteableData
	{
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		var tasks = set.Data.
			Select(data => DataWriteAs(
				data, 
				Path.Combine(path, $"{data.Name}{extension}")))
			.ToList();
		await Task.WhenAll(tasks);
	}

	public async Task SetFullDepthWriteAs<TData>(DataSet<TData> root, string path, string extension)
		where TData : IWriteableData
	{
		var track = LinkSetAndOutputFolder(root, path);
		var tasks = track.Keys
			.Select(set => Task.Run(() => SetOnlyWriteAs(
				set,
				track[set],
				extension)))
			.ToList();
		await Task.WhenAll(tasks);
	}

	private static Dictionary<DataSet<TData>, string> LinkSetAndOutputFolder<TData>(DataSet<TData> set, string path)
		where TData : IWriteableData
	{
		var track = new Dictionary<DataSet<TData>, string> {[set] = path};
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