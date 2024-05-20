using Domain.InputOutput;

namespace DataSource.InputOutput;

public class FileWriter(FileMode mode) : IDataWriter
{
	private  readonly FileStreamOptions options = new FileStreamOptions
	{
		Mode = mode,
		Access = FileAccess.Write,
	};

	public void WriteData(IWriteableData data, string fullName)
	{
		using var writer = new StreamWriter(fullName, options);
		foreach (var line in data.ToContents())
		{
			writer.WriteLine(line);
		}
	}
}