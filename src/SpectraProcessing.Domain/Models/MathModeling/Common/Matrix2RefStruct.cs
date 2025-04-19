using System.Text;
using SpectraProcessing.Domain.Extensions;

namespace SpectraProcessing.Domain.Models.MathModeling.Common;

public readonly ref struct Matrix2RefStruct
{
    private readonly Span2<float> values;

    public int RowsCount => values.RowsCount;

    public int ColumnsCount => values.ColumnsCount;

    public float this[int row, int column]
    {
        get => values[row, column];
        set => values[row, column] = value;
    }

    public Matrix2RefStruct(int rowsCount, int columnsCount, Span<float> buffer)
    {
        var length = rowsCount * columnsCount;

        if (buffer.Length < length)
        {
            throw new ArgumentException("Buffer is too small.");
        }

        values = new Span2<float>(rowsCount, columnsCount, buffer[..length]);
    }

    public Matrix2RefStruct Add(in Matrix2RefStruct other, float otherMultiplier = 1)
    {
        if (CanStack(this, other) is false)
        {
            throw new InvalidOperationException("Cannot stack into this matrix");
        }

        if (otherMultiplier.ApproximatelyEqual(1))
        {
            for (var row = 0; row < RowsCount; row++)
            {
                for (var column = 0; column < ColumnsCount; column++)
                {
                    values[row, column] += other[row, column];
                }
            }
        }
        else
        {
            for (var row = 0; row < RowsCount; row++)
            {
                for (var column = 0; column < ColumnsCount; column++)
                {
                    values[row, column] += otherMultiplier * other[row, column];
                }
            }
        }

        return this;
    }

    public Matrix2RefStruct Add(float thisMultiplier, in Matrix2RefStruct other)
    {
        if (CanStack(this, other) is false)
        {
            throw new InvalidOperationException("Cannot stack into this matrix");
        }

        if (thisMultiplier.ApproximatelyEqual(1))
        {
            for (var row = 0; row < RowsCount; row++)
            {
                for (var column = 0; column < ColumnsCount; column++)
                {
                    values[row, column] += other[row, column];
                }
            }
        }
        else
        {
            for (var row = 0; row < RowsCount; row++)
            {
                for (var column = 0; column < ColumnsCount; column++)
                {
                    values[row, column] = thisMultiplier * values[row, column] + other[row, column];
                }
            }
        }

        return this;
    }

    public Matrix2RefStruct Multiply(float multiplier)
    {
        for (var row = 0; row < RowsCount; row++)
        {
            for (var column = 0; column < ColumnsCount; column++)
            {
                values[row, column] *= multiplier;
            }
        }

        return this;
    }

    public static Matrix2RefStruct Multiply(in Matrix2RefStruct left, in Matrix2RefStruct right, in Span<float> buffer)
    {
        if (CanMultiply(left, right) is false)
        {
            throw new InvalidOperationException("Cannot multiply into this matrix");
        }

        var rows = left.RowsCount;
        var columns = right.ColumnsCount;
        var length = rows * columns;

        if (buffer.Length < length)
        {
            throw new InvalidOperationException("Buffer is too small");
        }

        var matrix = new Matrix2RefStruct(rows, columns, buffer[..length]);

        for (var row = 0; row < matrix.RowsCount; row++)
        {
            for (var column = 0; column < matrix.ColumnsCount; column++)
            {
                for (var i = 0; i < left.ColumnsCount; i++)
                {
                    matrix[row, column] += left[row, i] * right[i, column];
                }
            }
        }

        return matrix;
    }

    public static Matrix2RefStruct Transpose(in Matrix2RefStruct matrix, in Span<float> buffer)
    {
        var transposedRows = matrix.ColumnsCount;
        var transposedColumns = matrix.RowsCount;
        var length = transposedRows * transposedColumns;

        if (buffer.Length < length)
        {
            throw new InvalidOperationException("Buffer is too small");
        }

        var transposed = new Matrix2RefStruct(transposedRows, transposedColumns, buffer[..length]);

        for (var row = 0; row < matrix.RowsCount; row++)
        {
            for (var column = 0; column < matrix.ColumnsCount; column++)
            {
                transposed[column, row] = matrix[row, column];
            }
        }

        return transposed;
    }

    private static bool CanStack(in Matrix2RefStruct left, in Matrix2RefStruct right)
        => left.RowsCount == right.RowsCount && left.ColumnsCount == right.ColumnsCount;

    private static bool CanMultiply(Matrix2RefStruct left, Matrix2RefStruct right)
        => left.ColumnsCount == right.RowsCount;

    public override string ToString()
    {
        var builder = new StringBuilder();

        for (var row = 0; row < RowsCount; row++)
        {
            builder.Append($"({values[row, 0]:0.##}");

            for (var column = 1; column < ColumnsCount; column++)
            {
                builder.Append($"\t, {values[row, column]:0.##}");
            }

            builder.Append(")\n");
        }

        return builder.ToString();
    }
}
