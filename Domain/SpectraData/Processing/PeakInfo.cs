namespace Domain.SpectraData.Processing;

public record PeakInfo(Spectra Spectra, float XStart, float XEnd, float Square, float Height)
{
	public override string ToString()
		=> $"{Spectra.Name} {Square: 0.000} {Height: 0.000} {XStart: 0.000} {XEnd: 0.000}";
}