﻿using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.MathModeling.Common;
using SpectraProcessing.Domain.Models.MathModeling.Peaks;

namespace SpectraProcessing.Domain.MathModeling.Peaks;

public static class NelderMead
{
    private static readonly Comparer<SimplexPoint> Comparer
        = Comparer<SimplexPoint>.Create((x, y) => x.Value.CompareTo(y.Value));

    public static Task<VectorN> GetOptimized(
        NedlerMeadModel model,
        Func<VectorNRefStruct, Span<float>, float> funcForMin)
    {
        return Task.Run(() => GetOptimizedInternal(model, funcForMin));
    }

    private static VectorN GetOptimizedInternal(
        NedlerMeadModel model,
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

        var iterationsWithLessThanDelta = 0;
        var previousBestValue = GetBestPoint().Value;
        var previousDelta = float.MaxValue;

        for (var repeat = 0; repeat < settings.RepeatsCount; repeat++)
        {
            consecutiveShrinks = 0;
            InitializeSimplexPoints(GetBestPoint().Vector, funcBuffer);

            for (var iteration = 0; iteration < settings.MaxIterationsCount; iteration++)
            {
                MoveSimplex(funcBuffer);

                var best = GetBestPoint();

                if (IsCriteriaReached(best))
                {
                    return best.Vector;
                }
            }
        }

        return simplexPoints[0].Vector;

        SimplexPoint GetBestPoint()
        {
            Array.Sort(simplexPoints, Comparer);

            return simplexPoints[0];
        }

        void InitializeSimplexPoints(VectorN startVector, in Span<float> funcBuffer)
        {
            Span<float> buffer = stackalloc float[dimension];

            startVector = startVector.WithConstraints(constraints);

            simplexPoints[0].Vector.Update(startVector);
            simplexPoints[0].Value = funcForMin(simplexPoints[0].Vector.ToVectorNRefStruct(buffer), funcBuffer);

            for (var d = 0; d < dimension; d++)
            {
                const float initialShiftPercent = 0.1f;

                var simplexPoint = simplexPoints[d + 1];

                var vector = simplexPoint.Vector;

                vector.Update(startVector);

                var m = Random.Shared.Next() % 2 == 0 ? -1 : 1;

                vector[d] = vector[d].ApproximatelyEqual(0, initialShiftPercent)
                    ? initialShiftPercent * m
                    : vector[d] * (1 + initialShiftPercent * m);

                vector = vector.WithConstraints(constraints);

                simplexPoint.Value = funcForMin(vector.ToVectorNRefStruct(buffer), funcBuffer);
            }
        }

        bool IsCriteriaReached(SimplexPoint best)
        {
            if (settings.MinAbsoluteValue is not null && best.Value.ApproximatelyEqual(settings.MinAbsoluteValue.Value))
            {
                return true;
            }

            if (settings.MaxConsecutiveShrinks is not null &&
                consecutiveShrinks > settings.MaxConsecutiveShrinks)
            {
                return true;
            }

            if (settings is not
                {
                    MinDeltaPercentageBetweenIterations: not null,
                    MaxIterationsWithDeltaLessThanMin: not null,
                })
            {
                return false;
            }

            var currentDelta = best.Value - previousBestValue;

            var tolerance = settings.MinDeltaPercentageBetweenIterations.Value;

            if (previousDelta.ApproximatelyEqual(currentDelta, tolerance))
            {
                iterationsWithLessThanDelta++;
            }
            else
            {
                iterationsWithLessThanDelta = 0;
            }

            previousBestValue = best.Value;
            previousDelta = currentDelta;

            return iterationsWithLessThanDelta >= settings.MaxIterationsWithDeltaLessThanMin;
        }

        void MoveSimplex(in Span<float> funcBuffer)
        {
            const int reflectionCoefficient = 1;
            const int expansionCoefficient = 2;
            const float contractionCoefficient = 0.5f;
            const float shrinkCoefficient = 0.5f;

            var best = simplexPoints[0];
            var worst = simplexPoints[^1];

            var center = simplexPoints
                .Take(simplexPoints.Length - 1)
                .Select(x => x.Vector)
                .Sum(stackalloc float[dimension])
                .Divide(simplexPoints.Length - 1);

            var reflected = VectorNRefStruct.Difference(center, worst.Vector, stackalloc float[dimension])
                .Multiply(reflectionCoefficient)
                .Add(center)
                .WithConstraints(constraints);

            var reflectedValue = funcForMin(reflected, funcBuffer);

            //ReflectedBetterThanBest
            if (reflectedValue < best.Value)
            {
                var expanded = VectorNRefStruct.Difference(reflected, center, stackalloc float[dimension])
                    .Multiply(expansionCoefficient)
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
                .Multiply(contractionCoefficient)
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
                    .Multiply(shrinkCoefficient)
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
                vector[d] = constraint.WithReflection(vector[d]);
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
                vector[d] = constraint.WithReflection(vector[d]);
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
