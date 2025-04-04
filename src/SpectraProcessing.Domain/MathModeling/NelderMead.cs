using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;

namespace SpectraProcessing.Domain.MathModeling;

public static class NelderMead
{
    private static readonly Comparer<SimplexPoint> Comparer
        = Comparer<SimplexPoint>.Create((x, y) => x.Value.CompareTo(y.Value));

    public static Task<VectorN> GetOptimized(
        VectorN start,
        Func<IReadOnlyVectorN, double> funcForMin,
        OptimizationSettings settings)
    {
        return Task.Run(() => GetOptimizedInternal(start, funcForMin, settings));

        static VectorN GetOptimizedInternal(
            VectorN start,
            Func<IReadOnlyVectorN, double> funcForMin,
            OptimizationSettings settings)
        {
            var simplexPoints = GetSimplexPoints(start, funcForMin, settings);

            var iteration = 0;
            var consecutiveShrinks = 0;
            var deltas = new List<double>();
            while (iteration < settings.MaxIterationsCount && !IsCriteriaReached())
            {
                var prevBest = simplexPoints[0].Value;
                MoveSimplex();
                var newBest = simplexPoints[0].Value;
                deltas.Add(newBest - prevBest);
                simplexPoints.Sort(Comparer);
                iteration++;
            }

            return simplexPoints[0].Vector;

            bool IsCriteriaReached()
            {
                if (settings.Criteria.AbsoluteValue is not null &&
                    simplexPoints[0].Value.ApproximatelyEqual(settings.Criteria.AbsoluteValue.Value))
                {
                    return true;
                }

                if (settings.Criteria.MaxConsecutiveShrinks is not null &&
                    consecutiveShrinks > settings.Criteria.MaxConsecutiveShrinks)
                {
                    return true;
                }

                return false;
            }

            void MoveSimplex()
            {
                var best = simplexPoints[0];
                var worst = simplexPoints[^1];

                var center = simplexPoints
                    .Take(simplexPoints.Count - 1)
                    .Select(x => x.Vector)
                    .ToArray()
                    .Sum()
                    .Divide(simplexPoints.Count - 1);

                var reflected = (center - worst.Vector)
                    .Multiply(settings.Coefficients.Reflection)
                    .Add(center);

                var reflectedValue = funcForMin(reflected);

                //ReflectedBetterThanBest
                if (reflectedValue < best.Value)
                {
                    var expanded = (reflected - center)
                        .Multiply(settings.Coefficients.Expansion)
                        .Add(center);

                    var expandedValue = funcForMin(expanded);

                    if (expandedValue < reflectedValue)
                    {
                        worst.Vector = expanded;
                        worst.Value = expandedValue;
                    }
                    else
                    {
                        worst.Vector = reflected;
                        worst.Value = reflectedValue;
                    }

                    consecutiveShrinks = 0;

                    return;
                }

                //ReflectedBetterThanSecondWorst
                if (reflectedValue < simplexPoints[^2].Value)
                {
                    worst.Vector = reflected;
                    worst.Value = reflectedValue;
                    consecutiveShrinks = 0;

                    return;
                }

                var contracted = (worst.Vector - center)
                    .Multiply(settings.Coefficients.Contraction)
                    .Add(center);

                var contractedValue = funcForMin(contracted);

                //Contraction
                if (contractedValue < worst.Value)
                {
                    worst.Vector = contracted;
                    worst.Value = contractedValue;
                    consecutiveShrinks = 0;

                    return;
                }

                //Shrink
                consecutiveShrinks++;
                foreach (var point in simplexPoints.Skip(1))
                {
                    point.Vector
                        .Substract(best.Vector)
                        .Multiply(settings.Coefficients.Shrink)
                        .Add(best.Vector);

                    point.Value = funcForMin(point.Vector);
                }
            }
        }
    }

    private static List<SimplexPoint> GetSimplexPoints(
        VectorN start,
        Func<IReadOnlyVectorN, double> funcForMin,
        OptimizationSettings settings)
    {
        var simplexPoints = new List<SimplexPoint>(start.Dimension + 1)
        {
            new() { Vector = start, Value = funcForMin(start) },
        };

        for (var d = 0; d < start.Dimension; d++)
        {
            var newVector = new VectorN(start.Values);

            newVector[d] = newVector[d].ApproximatelyEqual(0)
                ? settings.InitialShift
                : newVector[d] * (1 + settings.InitialShift);

            simplexPoints.Add(new SimplexPoint { Vector = newVector, Value = funcForMin(newVector) });
        }

        simplexPoints.Sort(Comparer);

        return simplexPoints;
    }

    private sealed record SimplexPoint
    {
        public required VectorN Vector { get; set; }

        public required double Value { get; set; }
    }
}
