namespace Domain.SpectraData;

public abstract class PeakBorder {
	public float Left { get; init; }
	public float Rigth { get; init; }

	protected PeakBorder(float left, float right) {
		if (left > right) (left, right) = (right, left);
		Left = left;
		Rigth = right;
	}

	public abstract SpectraPlot GetPlot();

	public override bool Equals(object? obj) {
		return obj is PeakBorder border && Math.Abs(Left - border.Left) < 1e-6 && Math.Abs(Rigth - border.Rigth) < 1e-6;
	}

	public override int GetHashCode() => HashCode.Combine(Left, Rigth);

	public override string ToString() => $"{Left: #.###}:{Rigth: #.###}";
}