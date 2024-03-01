namespace Model.DataFormats;
public abstract class Plot : Data {
    public Spectra Spectra { get; init; }

    public Plot(Spectra spectra)
        : base(spectra.Name) {
        Spectra = spectra;
    }

    public override int GetHashCode() => Spectra.GetHashCode();
}
