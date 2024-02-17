namespace Model.DataFormats;
public abstract class Plot : Data {
    public Spectra Spectra { get; protected set; } = null!;

    public Plot(Spectra spectra)
        : base(spectra.Name) {
        Spectra = spectra;
    }

    public override int GetHashCode() => Spectra.GetHashCode();
}
