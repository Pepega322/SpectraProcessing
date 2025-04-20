// using FluentAssertions;
// using SpectraProcessing.Domain.MathModeling.Baseline.AirPLS;
// using SpectraProcessing.Domain.Models.MathModeling.Baseline;
// using SpectraProcessing.Domain.Models.MathModeling.Common;
// using SpectraProcessing.TestingInfrastructure;
// using Xunit;
// using Xunit.Abstractions;
//
// namespace SpectraProcessing.Tests.Tests;
//
// public class AirPlsTests
// {
//     private readonly ITestOutputHelper testOutputHelper;
//
//     public AirPlsTests(ITestOutputHelper testOutputHelper)
//     {
//         this.testOutputHelper = testOutputHelper;
//     }
//
//     [Fact]
//     public async Task Gauss()
//     {
//         var airPlsBaselineCorrectionModel = new AirPLSSettings
//         {
//             IterationsCount = 10,
//             SmoothCoefficient = 1e15f,
//             SmoothingTolerance = 1e-3f,
//         };
//
//         var baseline = await AirPLS.GetBaseline(ModelSpectras.Gauss, airPlsBaselineCorrectionModel);
//
//     }
//     [Fact]
//     public async Task Test()
//     {
//         float[] vector = [0, 1, 2, 3, 5, 10, 20, 100, 1000, 3000, 1000, 100, 20, 10, 5, 3, 2, 1, 0];
//
//         var airPlsBaselineCorrectionModel = new AirPLSSettings
//         {
//             IterationsCount = 10,
//             SmoothCoefficient = 100,
//             SmoothingTolerance = 1e-3f,
//         };
//
//         var baseline = await AirPLS.GetBaseline(vector, airPlsBaselineCorrectionModel);
//
//         baseline.ToString().Should().Be(t);
//
//     }
//
//     private const string t = "(-121,514206, -78,594955, -34,460564, 12,900029, 66,58927, 128,85823, 198,27467, 268,0073, 324,50885, 347,76306, 324,50873, 268,0071, 198,27446, 128,85803, 66,58915, 12,900025, -34,46042, -78,59464, -121,51373)";
//     // [Theory]
//     // [InlineData(1000)]
//     // public async Task CorrectBaseline_Success(int columns)
//     // {
//     //     var weight = new VectorNRefStruct(Enumerable.Repeat(1f, columns).ToArray());
//     //
//     //     // var penaltyMatrix = new Matrix2RefStruct(columns, columns, stackalloc float[columns * columns]);
//     //     // BuildFakeDtdMatrix(penaltyMatrix, columns);
//     //     //
//     //     // for (int i = 0; i < columns; i++)
//     //     // {
//     //     //     penaltyMatrix[i, i] += 1;
//     //     // }
//     //     //
//     //     //
//     //     // var realL = AirPLSBaselineCorrection.GetLMatrix(penaltyMatrix, stackalloc float[columns * columns]);
//     //     // testOutputHelper.WriteLine(realL.ToString());
//     //     var trickyL = AirPLSBaselineCorrection.LMatrix.Create(
//     //         weight,
//     //         new AirPLSBaselineCorrection.PenaltyMatrix(columns),
//     //         1f,
//     //         stackalloc float[3 * columns - 3]);
//     //
//     //     var b = new StringBuilder();
//     //
//     //     for (int row = 0; row < columns; row++)
//     //     {
//     //         for (int col = 0; col < columns; col++)
//     //         {
//     //             b.Append(trickyL[row, col] + " ");
//     //         }
//     //
//     //         b.AppendLine();
//     //         b.AppendLine();
//     //     }
//     //
//     //     testOutputHelper.WriteLine(b.ToString());
//     //
//     //     // realL.ToString().Should().Be(realTrickyL.ToString());
//     // }
//
//     //     [Theory]
//     //     [InlineData(10)]
//     //     public async Task CorrectBaseline_Success(int columns)
//     //     {
//     //         var matrix = new Matrix2RefStruct(columns, columns, stackalloc float[columns * columns]);
//     //         BuildFakeDtdMatrix(matrix, columns);
//     //
//     //         for (int i = 0; i < columns; i++)
//     //         {
//     //             matrix[i, i] += 1;
//     //         }
//     //
//     //         var l = AirPLSBaselineCorrection.GetLMatrix(matrix, stackalloc float[columns * columns]);
//     //
//     // testOutputHelper.WriteLine(l.ToString());
//     //
//     //         var d0 = new List<float>();
//     //
//     //         for (int i = 0; i < columns; i++)
//     //         {
//     //             d0.Add(l[i, i]);
//     //         }
//     //
//     //
//     //         var d1 = new List<float>();
//     //
//     //         for (int i = 0; i < columns - 1; i++)
//     //         {
//     //             d1.Add(l[i + 1, i]);
//     //         }
//     //
//     //         var d2 = new List<float>();
//     //
//     //         for (int i = 0; i < columns - 2; i++)
//     //         {
//     //             d2.Add(l[i + 2, i]);
//     //         }
//     //
//     //         var d3 = new List<float>();
//     //
//     //         for (int i = 0; i < columns - 3; i++)
//     //         {
//     //             d3.Add(l[i + 3, i]);
//     //         }
//     //
//     //
//     //         testOutputHelper.WriteLine(matrix.ToString());
//     //
//     //         l.ToString().Should().Be(LFor10);
//     //
//     //         var transpL = Matrix2RefStruct.Transpose(l, stackalloc float[l.RowsCount * l.ColumnsCount]);
//     //
//     //         var m = Matrix2RefStruct.Multiply(l, transpL, stackalloc float[columns * columns]);
//     //
//     //         testOutputHelper.WriteLine(m.ToString());
//     //     }
//
//     // [Theory]
//     // [InlineData(10)]
//     // public async Task CorrectBaseline_Success(int columns)
//     // {
//     //     var fake = new Matrix2RefStruct(columns, columns, stackalloc float[columns * columns]);
//     //     BuildFakeDtdMatrix(fake, columns);
//     //
//     //     testOutputHelper.WriteLine(fake.ToString());
//     //
//     //     // var real = new Matrix2RefStruct(columns, columns, stackalloc float[columns * columns]);
//     //     // BuildTrueDtdMatrix(real, columns);
//     //
//     //     // fake.ToString().Should().Be(real.ToString());
//     // }
//
//     private static void BuildFakeDtdMatrix(in Matrix2RefStruct empty, int columns)
//     {
//         var f = new AirPLS.PenaltyMatrix(columns);
//         for (var i = 0; i < columns; i++)
//         {
//             for (var j = 0; j < columns; j++)
//             {
//                 empty[i, j] = f[i, j];
//             }
//         }
//     }
//
//     // private static void BuildTrueDtdMatrix(in Matrix2RefStruct empty, int columns)
//     // {
//     //     var rows = columns - 2;
//     //
//     //     var d = new Matrix2RefStruct(rows, columns, stackalloc float[columns * rows]);
//     //
//     //     for (var i = 1; i < columns - 1; i++)
//     //     {
//     //         d[i - 1, i - 1] = 1;
//     //         d[i - 1, i] = -2;
//     //         d[i - 1, i + 1] = 1;
//     //     }
//     //
//     //     var dT = Matrix2RefStruct.Transpose(d, stackalloc float[columns * rows]);
//     //
//     //     var multiply = Matrix2RefStruct.Multiply(dT, d, stackalloc float[columns * columns]);
//     //
//     //     for (var i = 0; i < columns; i++)
//     //     {
//     //         for (var j = 0; j < columns; j++)
//     //         {
//     //             empty[i, j] = multiply[i, j];
//     //         }
//     //     }
//     // }
//
//     private const string LFor10 =
//         "(1,41\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00)\n(-1,41\t;\t2,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00)\n(0,71\t;\t-1,50\t;\t2,06\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00)\n(0,00\t;\t0,50\t;\t-1,58\t;\t2,07\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00)\n(0,00\t;\t0,00\t;\t0,49\t;\t-1,57\t;\t2,08\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00)\n(0,00\t;\t0,00\t;\t0,00\t;\t0,48\t;\t-1,56\t;\t2,08\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00)\n(0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,48\t;\t-1,56\t;\t2,08\t;\t0,00\t;\t0,00\t;\t0,00)\n(0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,48\t;\t-1,56\t;\t2,08\t;\t0,00\t;\t0,00)\n(0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,48\t;\t-1,56\t;\t1,82\t;\t0,00)\n(0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,00\t;\t0,48\t;\t-0,68\t;\t1,14)\n";
// }
