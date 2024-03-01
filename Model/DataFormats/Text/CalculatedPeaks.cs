using System.Collections;

namespace Model.DataFormats;
public class CalculatedPeaks : Data, IWriteable, IEnumerable<PeakInfo> {
    private List<PeakInfo> peaks = new();

    public CalculatedPeaks(string name)
        : base(name) { }

    public void Add(PeakInfo record) {
        lock (peaks)
            peaks.Add(record);
    }

    public IEnumerable<string> ToContents() {
        yield return "Name Square Heigth Start End";
        foreach (var info in peaks
            .OrderBy(p => p.XStart)
            .ThenBy(p => p.XEnd)
            .ThenBy(p => p.Spectra.Name))
            yield return info.ToString();
    }

    public IEnumerator<PeakInfo> GetEnumerator() {
        foreach (var peak in peaks)
            yield return peak;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
