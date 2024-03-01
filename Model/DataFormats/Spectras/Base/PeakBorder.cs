namespace Model.DataFormats;
public abstract record PeakBorder {
    public float Left { get; init; }
    public float Rigth { get; init; }

    protected PeakBorder(float left, float right) {
        if (left > right) (left, right) = (right, left);
        Left = left;
        Rigth = right;
    }

    //public override bool Equals(object? obj) {
    //    if (obj is not PeakBorder b) return false;
    //    return Left.Equals(b.Left) && Rigth.Equals(b.Rigth);
    //}

    public override int GetHashCode() => HashCode.Combine(Left, Rigth);

    public override string ToString() => $"{Left: #.###}:{Rigth: #.###}";
}
