namespace Domain.DataSource;

public interface IDataWriter
{
	void WriteData(IWriteable data, string path);
}