using Domain.SpectraData;
using Scott.GraphicsData;

namespace Scott.Data;

public class ScottPeakBorder(float left, float right) : PeakBorder(left, right) {
	private SpectraPlot? plottable;

	public override SpectraPlot GetPlot() {
		plottable ??= new PlottablePeakBorder(this);
		return plottable;
	}
}