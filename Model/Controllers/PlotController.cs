using Model.DataFormats;
using Model.DataStorages;
using Model.MathHelper;

namespace Model.Controllers;
public abstract class PlotController {
    protected PlotStorage storage { get; init; }
    protected PeakBordersController bordersControler { get; init; }
    public Point<float> Coordinates => bordersControler.Coordinates;

    protected PlotController(PlotStorage storage, PeakBordersController bordersControler) {
        this.storage = storage;
        this.bordersControler = bordersControler;
    }

    public abstract Task AddDataPlotAsync(Data data);

    public abstract Task AddDataSetPlotAsync(DataSet set, bool toDefault);

    public abstract Task RemovePlotAsync(Plot plot, PlotSet owner);

    public abstract Task RemovePlotSetAsync(PlotSet set);

    public abstract Task ChangePlotVisibilityAsync(Plot plot, bool isVisible);

    public abstract Task ChangePlotSetVisibilityAsync(PlotSet set, bool isVisible);

    public abstract Task ChangePlotHighlightionAsync(Plot plot);

    public abstract Task ChangePlotSetHighlightionAsync(PlotSet set);

    public abstract Task<CalculatedPeaks> ProcessPlot(Plot plot);

    public abstract Task<CalculatedPeaks> ProcessPlotSet(PlotSet set);

    public abstract void Refresh();

    public abstract void Resize();

    public abstract void Clear();

    public abstract Task SetCoordinates(float xScreen, float yScreen);

    public abstract Task<bool> AddBorder();

    public abstract Task<bool> DeleteLastBorder();

    public abstract Task<bool> ClearBorders();
}
