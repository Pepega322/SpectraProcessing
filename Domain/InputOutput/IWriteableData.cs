namespace Domain.InputOutput;

public interface IWriteableData
{
	string? Name { get; }
	string? Extension { get; }
	IEnumerable<string> ToContents();
}