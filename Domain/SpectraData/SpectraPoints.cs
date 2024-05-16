using Domain.SpectraData.Processing;

namespace Domain.SpectraData;

public record SpectraPoints
{
	public IReadOnlyList<float> X { get; init; }
	public IReadOnlyList<float> Y { get; init; }
	public int Count => X.Count;

	public Point<float> this[int index] => index < 0 || index >= X.Count
		? throw new ArgumentOutOfRangeException(nameof(index))
		: new Point<float>(X[index], Y[index]);

	public SpectraPoints(IList<float> x, IList<float> y)
	{
		if (x.Count != y.Count)
			throw new ArgumentException("Different points count in x and y collections");
		X = x.AsReadOnly();
		Y = y.AsReadOnly();
	}

	public SpectraPoints Transform(Func<float, float, float> transformRule)
	{
		var transformedY = new List<float>();
		for (var i = 0; i < Count; i++)
			transformedY.Add(transformRule(X[i], Y[i]));
		return new SpectraPoints((IList<float>) X, transformedY);
	}

	public IEnumerable<string> ToContents()
	{
		for (var i = 0; i < Count; i++)
			yield return $"{X[i]} {Y[i]}";
	}
}