namespace SpectraProcessing.Domain.Models.MathModeling.Common;

public readonly ref struct Span2<T>
{
    private readonly Span<T> values;

    public readonly int RowsCount;

    public readonly int ColumnsCount;

    public int Length => values.Length;

    public T this[int row, int column]
    {
        get => values[ColumnsCount * row + column];
        set => values[ColumnsCount * row + column] = value;
    }

    public Span2(int rowsCount, int columnsCount, Span<T> buffer)
    {
        var length = rowsCount * columnsCount;

        if (buffer.Length < length)
        {
            throw new ArgumentException("The buffer is too small.");
        }

        RowsCount = rowsCount;
        ColumnsCount = columnsCount;
        values = buffer[..length];
    }

    public Span2<T> Slice(int rows, int columns)
    {
        return new Span2<T>(rows, columns, values[..(rows * columns)]);
    }
}
