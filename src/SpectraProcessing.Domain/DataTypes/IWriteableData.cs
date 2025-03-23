namespace SpectraProcessing.Domain.DataTypes;

public interface IWriteableData
{
    string? Name { get; }

    string? Extension { get; }

    IEnumerable<string> ToContents();
}
