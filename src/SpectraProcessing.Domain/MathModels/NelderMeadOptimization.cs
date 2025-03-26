using SpectraProcessing.Domain.Models.MathModels;

namespace SpectraProcessing.Domain.MathModels;

public static class NelderMeadOptimization
{
    public static Task<VectorN> Optimize(
        VectorN start,
        Func<VectorN, double> func,
        NelderMeadOptimizationSettings settings)
    {

        throw new NotImplementedException();
    }
}

public sealed record NelderMeadOptimizationSettings
{

}
