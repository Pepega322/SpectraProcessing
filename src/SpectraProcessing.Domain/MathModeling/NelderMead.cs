using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling;

namespace SpectraProcessing.Domain.MathModeling;

public static class NelderMead
{
    private static readonly Comparer<SimplexPoint> Comparer
        = Comparer<SimplexPoint>.Create((x, y) => x.Value.CompareTo(y.Value));

    public static Task<VectorN> GetOptimized(
        NedlerMeadOptimizationModel model,
        Func<VectorNRefStruct, Span<float>, float> funcForMin)
    {
        return Task.Run(() => GetOptimizedInternal(model, funcForMin));
    }

    private static VectorN GetOptimizedInternal(
        NedlerMeadOptimizationModel model,
        Func<VectorNRefStruct, Span<float>, float> funcForMin)
    {
        Span<float> funcBuffer = stackalloc float[model.BufferSize];

        var dimension = model.Start.Dimension;
        var settings = model.Settings;
        var constraints = model.Constraints;

        var simplexPoints = GetSimplexPoints(funcBuffer);

        var iteration = 0;
        var consecutiveShrinks = 0;
        while (iteration < settings.MaxIterationsCount && !IsCriteriaReached())
        {
            MoveSimplex(funcBuffer);
            Array.Sort(simplexPoints, Comparer);
            iteration++;
        }

        return simplexPoints.First().Vector;


        SimplexPoint[] GetSimplexPoints(in Span<float> funcBuffer)
        {
            Span<float> buffer = stackalloc float[dimension];

            var start = model.Start.WithConstraints(constraints);

            var points = new SimplexPoint[dimension + 1];

            points[0] = new SimplexPoint
            {
                Vector = start,
                Value = funcForMin(start.ToVectorNRefStruct(buffer), funcBuffer),
            };

            for (var d = 0; d < dimension; d++)
            {
                var newValues = start.Values.ToArray();

                var m = Random.Shared.Next() % 2 == 0 ? -1 : 1;

                newValues[d] = newValues[d].ApproximatelyEqual(0)
                    ? settings.InitialShift * m
                    : newValues[d] * (1 + settings.InitialShift * m);

                var newVector = new VectorN(newValues)
                    .WithConstraints(constraints);

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
                .Sum(stackalloc float[dimension])
                .Divide(simplexPoints.Length - 1);

            var reflected = VectorNRefStruct.Difference(center, worst.Vector, stackalloc float[dimension])
                .Multiply(settings.Coefficients.Reflection)
                .Add(center)
                .WithConstraints(constraints);

            var reflectedValue = funcForMin(reflected, funcBuffer);

            //ReflectedBetterThanBest
            if (reflectedValue < best.Value)
            {
                var expanded = VectorNRefStruct.Difference(reflected, center, stackalloc float[dimension])
                    .Multiply(settings.Coefficients.Expansion)
                    .Add(center)
                    .WithConstraints(constraints);

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

            var contracted = VectorNRefStruct.Difference(worst.Vector, center, stackalloc float[dimension])
                .Multiply(settings.Coefficients.Contraction)
                .Add(center)
                .WithConstraints(constraints);

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
            Span<float> buffer = stackalloc float[dimension];
            foreach (var point in simplexPoints.Skip(1))
            {
                var shrinked = VectorNRefStruct.Difference(point.Vector, best.Vector, buffer)
                    .Multiply(settings.Coefficients.Shrink)
                    .Add(best.Vector)
                    .WithConstraints(constraints);

                point.Value = funcForMin(shrinked, funcBuffer);
                point.Vector.Update(shrinked);

                buffer.Clear();
            }

            consecutiveShrinks++;
        }
    }

    private static VectorN WithConstraints(
        this VectorN vector,
        Dictionary<int, ValueConstraint> constraints)
    {
        for (var d = 0; d < vector.Dimension; d++)
        {
            if (constraints.TryGetValue(d, out var constraint))
            {
                constraint.ApplyWithReflection(ref vector.Values[d]);
            }
        }

        return vector;
    }

    private static VectorNRefStruct WithConstraints(
        this in VectorNRefStruct vector,
        Dictionary<int, ValueConstraint> constraints)
    {
        for (var d = 0; d < vector.Dimension; d++)
        {
            if (constraints.TryGetValue(d, out var constraint))
            {
                constraint.ApplyWithReflection(ref vector.Values[d]);
            }
        }

        return vector;
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
