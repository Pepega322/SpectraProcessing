using Domain.DataSource;

namespace DataSource.FileSource;

public class FileWriter : IDataWriter {
	public void WriteData(IWriteable data, string fullName) {
		var file = File.Create(fullName);
		file.Close();
		File.AppendAllLines(fullName, data.ToContents());
	}
}