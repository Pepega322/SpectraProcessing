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

        var simplexPoints = Enumerable.Range(0, dimension + 1)
            .Select(_ => new SimplexPoint { Vector = new VectorN(dimension), Value = 0 })
            .ToArray();

        InitializeSimplexPoints(model.Start, funcBuffer);

        int consecutiveShrinks;
        for (var repeat = 0; repeat < settings.RepeatsCount; repeat++)
        {
            consecutiveShrinks = 0;
            InitializeSimplexPoints(simplexPoints[0].Vector, funcBuffer);

            for (var iteration = 0; iteration < settings.MaxIterationsCount; iteration++)
            {
                MoveSimplex(funcBuffer);
                Array.Sort(simplexPoints, Comparer);

                if (IsCriteriaReached())
                {
                    return simplexPoints[0].Vector;
                }
            }
        }

        return simplexPoints[0].Vector;

        void InitializeSimplexPoints(VectorN startVector, in Span<float> funcBuffer)
        {
            Span<float> buffer = stackalloc float[dimension];

            startVector = startVector.WithConstraints(constraints);

            simplexPoints[0].Vector.Update(startVector);
            simplexPoints[0].Value = funcForMin(simplexPoints[0].Vector.ToVectorNRefStruct(buffer), funcBuffer);

            for (var d = 0; d < dimension; d++)
            {
                var simplexPoint = simplexPoints[d + 1];

                var vector = simplexPoint.Vector;

                vector.Update(startVector);

                var m = Random.Shared.Next() % 2 == 0 ? -1 : 1;

                vector[d] = vector[d].ApproximatelyEqual(0)
                    ? settings.InitialShift * m
                    : vector[d] * (1 + settings.InitialShift * m);

                vector = vector.WithConstraints(constraints);

                simplexPoint.Value = funcForMin(vector.ToVectorNRefStruct(buffer), funcBuffer);
            }

            Array.Sort(simplexPoints, Comparer);
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
