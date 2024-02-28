namespace Model.DataFormats;
public record PeakInfo(Spectra Spectra, float XStart, float XEnd, float Square, float Heigth) {
    public override string ToString()
        => $"{Spectra.Name} {Square: #.###} {Heigth: #.###} {XStart: #.###} {XEnd: #.###}";
}