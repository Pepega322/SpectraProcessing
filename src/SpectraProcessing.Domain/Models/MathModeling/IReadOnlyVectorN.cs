namespace SpectraProcessing.Domain.Models.MathModeling;

public interface IReadOnlyVectorN
{
    IReadOnlyList<double> Values { get; }

    int Dimension { get; }
}
