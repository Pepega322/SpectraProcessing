using Domain.DataSource;

namespace Domain.SpectraData.ProcessingInfo;

public class PeakInfoSet : Data, IWriteable {
	private readonly List<PeakInfo> peaks = [];

	public string Name => string.Empty;

	public void AddThreadSafe(PeakInfo record) {
		lock (peaks) peaks.Add(record);
	}

	public IEnumerable<string> ToContents() {
		yield return "Name Square Heigth Start End";
		foreach (var info in peaks
			         .OrderBy(p => p.XStart)
			         .ThenBy(p => p.XEnd)
			         .ThenBy(p => p.Spectra.Name))
			yield return info.ToString();
	}
}