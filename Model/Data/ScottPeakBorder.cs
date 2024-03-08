using Domain.SpectraData;
using Scott.GraphicsData;

namespace Scott.Data;

public class ScottPeakBorder(float Left, float Right) : PeakBorder(Left, Right) {
    private SpectraPlot? plottable;

    public override SpectraPlot GetPlot() {
        plottable ??= new PlottablePeakBorder(this);
        return plottable;
    }
}
