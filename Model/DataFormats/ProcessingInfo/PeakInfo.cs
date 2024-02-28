namespace Model.DataFormats;
public record PeakInfo(Spectra Spectra, float XStart, float XEnd, float Square, float Heigth) {
    public override string ToString()
        => $"{Spectra.Name}\t{Square: #.###}\t{Heigth: #.###}\t{XStart: #.###}\t{XEnd: #.###}";
}