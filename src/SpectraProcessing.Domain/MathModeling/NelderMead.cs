using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;

namespace SpectraProcessing.Domain.MathModeling;

public static class NelderMead
{
    public static Task<VectorN> Optimization(
        VectorN start,
        Func<VectorN, double> func,
        OptimizationSettings settings)
    {
        return Task.Run(() => OptimizeInternal(start, func, settings));
    }

    private static VectorN OptimizeInternal(
        VectorN start,
        Func<VectorN, double> func,
        OptimizationSettings settings)
    {
        var simplex = new Simplex(start, func, settings.SimplexSettings);

        for (var i = 0; i < settings.MaxIterationsCount; i++)
        {
            Iteration(simplex, settings.Coefficients);

            if (IsCriteriaReached(simplex, settings.Сriteria))
            {
                break;
            }
        }

        return simplex.Best;
    }

    private static bool IsCriteriaReached(
        Simplex simplex,
        OptimizationSettings.CompletionСriteria criteria)
    {
        return criteria.AbsoluteValue is not null
            && simplex.BestValue.ApproximatelyEqual(criteria.AbsoluteValue.Value);
    }

    private static void Iteration(
        Simplex simplex,
        OptimizationSettings.IterationCoefficients coefficients)
    {
        var center = simplex.Center;

        var reflected = center + coefficients.Reflection * (center - simplex.Worst);
        var reflectedValue = simplex.GetFuncValue(reflected);

        //reflectedBetterThanBest
        if (reflectedValue < simplex.BestValue)
        {
            var expanded = center + coefficients.Expansion * (reflected - center);

            simplex.Worst = simplex.GetFuncValue(expanded) < reflectedValue
                ? expanded
                : reflected;

            return;
        }

        if (reflectedValue < simplex.SecondWorstValue)
        {
            simplex.Worst = reflected;
            return;
        }

        var contracted = center + coefficients.Contraction * (simplex.Worst - center);

        if (simplex.GetFuncValue(contracted) < simplex.WorstValue)
        {
            simplex.Worst = contracted;
            return;
        }

        simplex.Shrink(coefficients.Shrink);
    }
}
