using SpectraProcessing.Domain.Models.MathModeling.Common;

namespace SpectraProcessing.Domain.MathModeling.Baseline.AirPLS;

public static partial class AirPLS
{
    private readonly ref struct LMatrix
    {
        private readonly int mainDiagonalSize;
        private readonly Span<float> threeDiagonalsValues;

        public float this[int row, int column]
        {
            get
            {
                var index = GetIndex(row, column);
                return index is not null ? threeDiagonalsValues[index.Value] : 0;
            }
            set
            {
                var index = GetIndex(row, column);
                if (index is not null)
                {
                    threeDiagonalsValues[index.Value] = value;
                }
            }
        }

        private LMatrix(int mainDiagonalSize, Span<float> buffer)
        {
            this.mainDiagonalSize = mainDiagonalSize;
            threeDiagonalsValues = buffer[..(3 * mainDiagonalSize - 3)];
        }

        public static LMatrix Create(
            in VectorNRefStruct weightVector,
            in PenaltyMatrix penaltyMatrix,
            float smoothingCoefficient,
            Span<float> buffer)
        {
            if (weightVector.Dimension != penaltyMatrix.Size)
            {
                throw new ArgumentException("The specified vector is not the same size as the penalty matrix");
            }

            var size = weightVector.Dimension;
            var lMatrix = new LMatrix(weightVector.Dimension, buffer);

            for (var row = 0; row < size; row++)
            {
                var diagonalSquare =
                    weightVector[row] + smoothingCoefficient * penaltyMatrix[row, row];

                for (var column = row - 2; column < row; column++)
                {
                    if (column < 0)
                    {
                        continue;
                    }

                    diagonalSquare -= lMatrix[row, column] * lMatrix[row, column];
                }

                if (diagonalSquare <= 0)
                {
                    throw new InvalidOperationException("Matrix is not positive definite.");
                }

                lMatrix[row, row] = MathF.Sqrt(diagonalSquare);

                for (var tRow = row + 1; tRow < row + 3 && tRow < size; tRow++)
                {
                    var underlyingElement = smoothingCoefficient * penaltyMatrix[tRow, row];

                    for (var column = row - 1; column < row; column++)
                    {
                        if (column < 0)
                        {
                            continue;
                        }

                        underlyingElement -= lMatrix[tRow, column] * lMatrix[row, column];
                    }

                    lMatrix[tRow, row] = underlyingElement / lMatrix[row, row];
                }
            }

            return lMatrix;
        }

        private int? GetIndex(int row, int column)
        {
            if (row < 0 || column < 0 || row >= mainDiagonalSize || column >= mainDiagonalSize)
            {
                throw new IndexOutOfRangeException();
            }

            if (row - column == 0 && column < mainDiagonalSize)
            {
                return column;
            }

            if (row - column == 1 && column < mainDiagonalSize - 1)
            {
                return mainDiagonalSize + column;
            }

            if (row - column == 2 && column < mainDiagonalSize - 2)
            {
                return 2 * mainDiagonalSize - 1 + column;
            }

            return null;
        }
    }

    public readonly ref struct PenaltyMatrix(int size)
    {
        public readonly int Size = size;

        public int this[int row, int column] => GetPenaltyMatrixValue(row, column);

        private int GetPenaltyMatrixValue(int row, int column)
        {
            if ((row >= 0 && row < Size) is false || (column >= 0 && column < Size) is false)
            {
                throw new IndexOutOfRangeException();
            }

            switch (column)
            {
                case 0:
                    return row switch
                    {
                        0 => 1,
                        1 => -2,
                        2 => 1,
                        _ => 0,
                    };
                case 1:
                    return row switch
                    {
                        0 => -2,
                        1 => 5,
                        2 => -4,
                        3 => 1,
                        _ => 0,
                    };
            }

            var shift = column - row;

            if (column < Size - 2)
            {
                return shift switch
                {
                    -2 => 1,
                    -1 => -4,
                    0  => 6,
                    1  => -4,
                    2  => 1,
                    _  => 0,
                };
            }

            if (column == Size - 2)
            {
                return shift switch
                {
                    -1 => -2,
                    0  => 5,
                    1  => -4,
                    2  => 1,
                    _  => 0,
                };
            }

            if (column == Size - 1)
            {
                return shift switch
                {
                    0   => 1,
                    1   => -2,
                    2   => 1,
                    < 0 => throw new Exception("WTF"),
                    _   => 0,
                };
            }

            throw new Exception("WTF2");
        }
    }
}
