using Controllers.Interfaces;
using Domain.InputOutput;
using Domain.Storage;

namespace Controllers;

public sealed class DirectoryDataWriterController(IDataWriter writer) : IDataWriterController
{
	public void DataWriteAs<TData>(TData data, string path)
		where TData : IWriteableData
	{
		writer.WriteData(data, path);
	}

	public void SetOnlyWriteAs<TData>(DataSet<TData> set, string path, string extension)
		where TData : IWriteableData
	{
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		Parallel.ForEach(set, d => DataWriteAs(d, Path.Combine(path, $"{d.Name}{extension}")));
	}

	public void SetFullDepthWriteAs<TData>(DataSet<TData> root, string path, string extension)
		where TData : IWriteableData
	{
		var track = LinkSetAndOutputFolder(root, path);
		foreach (var set in track.Keys)
		{
			SetOnlyWriteAs(set, track[set], extension);
		}
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