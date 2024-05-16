using Domain.InputOutput;

namespace DataSource.InputOutput;

public class FileWriter : IDataWriter
{
	private static readonly FileStreamOptions Options = new FileStreamOptions
	{
		Mode = FileMode.CreateNew,
		Access = FileAccess.Write,
	};

	public void WriteData(IWriteableData data, string fullName)
	{
		using var writer = new StreamWriter(fullName, Options);
		foreach (var line in data.ToContents())
		{
			writer.WriteLine(line);
		}
	}
}