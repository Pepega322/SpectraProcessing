namespace Model.DataFormats;
public abstract class PeakBorder {
    public readonly float Left;
    public readonly float Rigth;

    protected PeakBorder(float left, float right) {
        if (left > right) (left, right) = (right, left);
        Left = left;
        Rigth = right;
    }
}
