using SpectraProcessing.Domain.Models.MathModeling;

namespace SpectraProcessing.Domain.Extensions;

public static class VectorNExtensions
{
    public static VectorNRefStruct Sum(this IEnumerable<VectorN> vectors, Span<float> buffer)
    {
        var vectorsArray = vectors.ToArray();

        var dimension = vectorsArray.Select(x => x.Dimension).Distinct().Single();

        for (var d = 0; d < dimension; d++)
        {
            buffer[d] = vectorsArray.Sum(v => v[d]);
        }

        return new VectorNRefStruct(buffer);
    }
}
