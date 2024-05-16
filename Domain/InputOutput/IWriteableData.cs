namespace Domain.InputOutput;

public interface IWriteableData
{
	string Name { get; }
	IEnumerable<string> ToContents();
}