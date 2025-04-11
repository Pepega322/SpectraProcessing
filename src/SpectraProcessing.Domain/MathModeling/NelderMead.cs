using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;

namespace SpectraProcessing.Domain.MathModeling;

public static class NelderMead
{
    private static readonly Comparer<SimplexPoint> Comparer
        = Comparer<SimplexPoint>.Create((x, y) => x.Value.CompareTo(y.Value));

    public static Task<VectorN> GetOptimized(
        VectorN start,
        Func<VectorNRefStruct, Span<float>, float> funcForMin,
        int bufferSize,
        OptimizationSettings settings)
    {
        return Task.Run(() => GetOptimizedInternal(start, funcForMin, bufferSize, settings));
    }

    private static VectorN GetOptimizedInternal(
        VectorN start,
        Func<VectorNRefStruct, Span<float>, float> funcForMin,
        int bufferSize,
        OptimizationSettings settings)
    {
        Span<float> funcBuffer = stackalloc float[bufferSize];

        var dimensions = start.Dimension;

        var simplexPoints = GetSimplexPoints(funcBuffer);

        var iteration = 0;
        var consecutiveShrinks = 0;
        while (iteration < settings.MaxIterationsCount && !IsCriteriaReached())
        {
            MoveSimplex(funcBuffer);
            Array.Sort(simplexPoints, Comparer);
            iteration++;
        }

        return simplexPoints[0].Vector;

        SimplexPoint[] GetSimplexPoints(in Span<float> funcBuffer)
        {
            Span<float> buffer = stackalloc float[start.Dimension];

            var points = new SimplexPoint[start.Dimension + 1];

            points[0] = new SimplexPoint
            {
                Vector = start,
                Value = funcForMin(start.ToVectorNRefStruct(buffer), funcBuffer),
            };

            for (var d = 0; d < start.Dimension; d++)
            {
                var newVector = new VectorN(start.Values.ToArray());

                var m = Random.Shared.Next() % 2 == 0 ? -1 : 1;

                newVector[d] = newVector[d].ApproximatelyEqual(0)
                    ? settings.InitialShift * m
                    : newVector[d] * (1 + settings.InitialShift * m);


                points[d + 1] = new SimplexPoint
                {
                    Vector = newVector,
                    Value = funcForMin(newVector.ToVectorNRefStruct(buffer), funcBuffer),
                };
            }

            Array.Sort(points, Comparer);

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

        void MoveSimplex(in Span<float> funcBuffer)
        {
            var best = simplexPoints[0];
            var worst = simplexPoints[^1];

            var center = simplexPoints
                .Take(simplexPoints.Length - 1)
                .Select(x => x.Vector)
                .Sum(stackalloc float[dimensions])
                .Divide(simplexPoints.Length - 1);

            var reflected = VectorNRefStruct.Difference(center, worst.Vector, stackalloc float[dimensions])
                .Multiply(settings.Coefficients.Reflection)
                .Add(center);

            var reflectedValue = funcForMin(reflected, funcBuffer);

            //ReflectedBetterThanBest
            if (reflectedValue < best.Value)
            {
                var expanded = VectorNRefStruct.Difference(reflected, center, stackalloc float[dimensions])
                    .Multiply(settings.Coefficients.Expansion)
                    .Add(center);

                var expandedValue = funcForMin(expanded, funcBuffer);

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

            var contractedValue = funcForMin(contracted, funcBuffer);

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

                point.Value = funcForMin(shrinked, funcBuffer);
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
