namespace Model.DataFormats;
public class Peak {
    private Spectra spectra;
    public float Start;
    public float End;
    public float Width;
    public float Heigth;
    public float Square;

    public Peak(Spectra spectra, float xStart, float xEnd, float heigth, float square) {
        this.spectra = spectra;
        Start = xStart;
        End = xEnd;
        Width = xEnd - xStart;
        Heigth = heigth;
        Square = square;
    }

    public override string ToString()
        => $"{spectra.Name} {Start} {End} {Width} {Heigth} {Square}";
}