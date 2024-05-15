namespace Domain.DataSource;

public interface IWriteable
{
	string Name { get; }
	IEnumerable<string> ToContents();
}