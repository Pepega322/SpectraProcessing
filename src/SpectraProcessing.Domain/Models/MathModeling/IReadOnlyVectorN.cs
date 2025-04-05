namespace SpectraProcessing.Domain.Models.MathModeling;

public interface IReadOnlyVectorN
{
    double this[int index] { get; }

    IReadOnlyList<double> Values { get; }

    int Dimension { get; }
}
