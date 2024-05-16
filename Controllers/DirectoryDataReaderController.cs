using Controllers.Interfaces;
using Controllers.Settings;
using Domain.InputOutput;
using Domain.Storage;
using Microsoft.Extensions.Options;

namespace Controllers;

public sealed class DirectoryDataReaderController<TData>(
	IOptions<DataReaderControllerSettings> settings,
	IDataReader<TData> reader
) : IDataReaderController<TData> where TData : class
{
	public DirectoryInfo Root { get; private set; } = new DirectoryInfo(settings.Value.StartFolderPath);

	public TData? Read(string fullName)
	{
		try
		{
			return reader.Get(fullName);
		}
		catch
		{
			return null;
		}
	}

	public DataSet<TData> ReadRoot()
	{
		var set = new DataSet<TData>(Root.Name);
		Parallel.ForEach(Root.GetFiles(), (file) =>
		{
			var data = Read(file.FullName);
			if (data != null)
				set.AddThreadSafe(data);
		});
		return set;
	}

	public DataSet<TData> ReadRootFullDepth()
	{
		var rootSet = new DataSet<TData>(Root.Name);
		var queue = new Queue<(DataSet<TData> Node, DirectoryInfo Directory)>();
		queue.Enqueue((rootSet, Root));
		while (queue.Count > 0)
		{
			var (node, dir) = queue.Dequeue();
			Parallel.ForEach(dir.GetFiles(), (file) =>
			{
				var data = Read(file.FullName);
				if (data != null)
					node.AddThreadSafe(data);
			});
			foreach (var subdirectory in dir.GetDirectories())
			{
				var subnode = new DataSet<TData>(subdirectory.Name);
				node.AddSubsetThreadSafe(subnode);
				queue.Enqueue((subnode, subdirectory));
			}
		}

		return rootSet;
	}

	public bool ChangeRoot(string path)
	{
		var newDir = new DirectoryInfo(path);
		if (!newDir.Exists) return false;
		Root = new DirectoryInfo(path);
		return true;
	}

	public bool StepBack()
	{
		return Root.Parent != null && ChangeRoot(Root.Parent.FullName);
	}
}