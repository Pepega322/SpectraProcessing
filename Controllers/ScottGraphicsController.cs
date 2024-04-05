using Domain;
using Domain.SpectraData;
using Scott.GraphicsData;

namespace Controllers;

public class ScottGraphicsController(ScottSpectraGraphics graphics) {
	private Spectra? highlighted;
	private DataSet<Spectra>? highlightedSet;

	public void Draw(Spectra spectra) {
		var plot = spectra.GetPlot();
		graphics.DrawThreadSafe(plot);
	}

	public void DrawSet(DataSet<Spectra> set) {
		Parallel.ForEach(set, Draw);
	}

	public void Erase(Spectra spectra) {
		var plot = spectra.GetPlot();
		graphics.EraseThreadSafe(plot);
	}

	public void EraseSet(DataSet<Spectra> set) {
		Parallel.ForEach(set, Erase);
	}

	public void ChangeVisibility(Spectra spectra, bool isVisible) {
		var plot = spectra.GetPlot();
		plot.ChangeVisibility(isVisible);
	}

	public void SetChangeVisibility(DataSet<Spectra> set, bool isVisible) {
		Parallel.ForEach(set, spectra => ChangeVisibility(spectra, isVisible));
	}

	public void Clear() {
		graphics.ClearThreadSafe();
		highlighted = null;
		highlightedSet = null;
	}

	public void SetChangeHighlightion(DataSet<Spectra> set) {
		if (highlightedSet != null)
			Parallel.ForEach(highlightedSet, spectra => ChangeHighlightion(spectra, false));

		if (highlightedSet == set) highlightedSet = null;
		else {
			highlightedSet = set;
			Parallel.ForEach(set, spectra => ChangeHighlightion(spectra, true));
		}
	}

	public void ChangeHighlightion(Spectra spectra) {
		if (highlighted != null)
			ChangeHighlightion(highlighted, false);

		if (highlighted == spectra) highlighted = null;
		else {
			highlighted = spectra;
			ChangeHighlightion(spectra, true);
		}
	}

	public void Resize() {
		graphics.ResizeThreadSafe();
	}

	private void ChangeHighlightion(Spectra spectra, bool isHighlighted) {
		var plot = spectra.GetPlot();
		graphics.ChangeHighlightion(plot, isHighlighted);
	}
}