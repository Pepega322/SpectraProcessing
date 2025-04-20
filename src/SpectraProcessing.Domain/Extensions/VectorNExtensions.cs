using SpectraProcessing.Domain.Models.MathModeling.Common;

namespace SpectraProcessing.Domain.Extensions;

public static class VectorNExtensions
{
    public static VectorNRefStruct Sum(this IEnumerable<VectorN> vectors, in Span<float> buffer)
    {
        buffer.Clear();

        foreach (var vector in vectors)
        {
            for (var d = 0; d < buffer.Length; d++)
            {
                buffer[d] += vector[d];
            }
        }

        return new VectorNRefStruct(buffer.Length, buffer);
    }
}
