namespace Domain.SpectraData.ProcessingInfo;

public record PeakInfo(Spectra Spectra, float XStart, float XEnd, float Square, float Heigth) {
	public override string ToString() =>
		$"{Spectra.Name} {Square: 0.000} {Heigth: 0.000} {XStart: 0.000} {XEnd: 0.000}";
}