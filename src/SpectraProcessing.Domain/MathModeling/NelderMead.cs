using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;

namespace SpectraProcessing.Domain.MathModeling;

public static class NelderMead
{
    private static readonly Comparer<SimplexPoint> Comparer
        = Comparer<SimplexPoint>.Create((x, y) => x.Value.CompareTo(y.Value));

    public static Task<VectorN> GetOptimized(
        VectorN start,
        Func<VectorNRefStruct, float> funcForMin,
        OptimizationSettings settings)
    {
        return Task.Run(() => GetOptimizedInternal(start, funcForMin, settings));
    }

    private static VectorN GetOptimizedInternal(
        VectorN start,
        Func<VectorNRefStruct, float> funcForMin,
        OptimizationSettings settings)
    {
        var dimensions = start.Dimension;

        var simplexPoints = GetSimplexPoints();

        var iteration = 0;
        var consecutiveShrinks = 0;
        while (iteration < settings.MaxIterationsCount && !IsCriteriaReached())
        {
            MoveSimplex();
            simplexPoints.Sort(Comparer);
            iteration++;
        }

        return simplexPoints[0].Vector;

        List<SimplexPoint> GetSimplexPoints()
        {
            Span<float> buffer = stackalloc float[start.Dimension];

            var points = new List<SimplexPoint>(start.Dimension + 1)
            {
                new()
                {
                    Vector = start,
                    Value = funcForMin(start.ToVectorNRefStruct(buffer))
                },
            };

            for (var d = 0; d < start.Dimension; d++)
            {
                var newVector = new VectorN(start.Values.ToArray());

                newVector[d] = newVector[d].ApproximatelyEqual(0)
                    ? settings.InitialShift
                    : newVector[d] * (1 + settings.InitialShift);

                newVector[d] *= 1
                    + (float) Random.Shared.NextDouble() * settings.InitialShift / 3f *
                    (Random.Shared.Next() % 2 == 0 ? -1 : 1);

                points.Add(
                    new SimplexPoint
                        { Vector = newVector, Value = funcForMin(newVector.ToVectorNRefStruct(buffer)) });
            }

            points.Sort(Comparer);

            return points;
        }

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
                .Sum(stackalloc float[dimensions])
                .Divide(simplexPoints.Count - 1);

            var reflected = VectorNRefStruct.Difference(center, worst.Vector, stackalloc float[dimensions])
                .Multiply(settings.Coefficients.Reflection)
                .Add(center);

            var reflectedValue = funcForMin(reflected);

            //ReflectedBetterThanBest
            if (reflectedValue < best.Value)
            {
                var expanded = VectorNRefStruct.Difference(reflected, center, stackalloc float[dimensions])
                    .Multiply(settings.Coefficients.Expansion)
                    .Add(center);

                var expandedValue = funcForMin(expanded);

                if (expandedValue < reflectedValue)
                {
                    worst.Vector.Update(expanded);
                    worst.Value = expandedValue;
                }
                else
                {
                    worst.Vector.Update(reflected);
                    worst.Value = reflectedValue;
                }

                consecutiveShrinks = 0;

                return;
            }

            //ReflectedBetterThanSecondWorst
            if (reflectedValue < simplexPoints[^2].Value)
            {
                worst.Vector.Update(reflected);
                worst.Value = reflectedValue;
                consecutiveShrinks = 0;

                return;
            }

            var contracted = VectorNRefStruct.Difference(worst.Vector, center, stackalloc float[dimensions])
                .Multiply(settings.Coefficients.Contraction)
                .Add(center);

            var contractedValue = funcForMin(contracted);

            //Contraction
            if (contractedValue < worst.Value)
            {
                worst.Vector.Update(contracted);
                worst.Value = contractedValue;
                consecutiveShrinks = 0;

                return;
            }

            //Shrink
            Span<float> buffer = stackalloc float[dimensions];
            foreach (var point in simplexPoints.Skip(1))
            {
                var shrinked = VectorNRefStruct.Difference(point.Vector, best.Vector, buffer)
                    .Multiply(settings.Coefficients.Shrink)
                    .Add(best.Vector);

                point.Value = funcForMin(shrinked);
                point.Vector.Update(shrinked);

                buffer.Clear();
            }

            consecutiveShrinks++;
        }
    }

    private sealed class SimplexPoint
    {
        public required VectorN Vector { get; init; }

        public required float Value { get; set; }

        public override string ToString()
        {
            return $"{Vector}, {Value}";
        }
    }
}
