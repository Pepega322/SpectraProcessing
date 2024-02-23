using Model.DataFormats;
using Model.DataStorages;

namespace Model.Controllers;
public abstract class PlotController {
    protected PlotStorage storage;

    protected PlotController(PlotStorage storage) {
        this.storage = storage;
    }

    public abstract Task AddDataPlotAsync(Data data);

    public abstract Task AddDataSetPlotAsync(DataSet set, bool toDefault);

    public abstract Task RemovePlotAsync(Plot plot, PlotSet owner);

    public abstract Task RemovePlotSetAsync(PlotSet set);

    public abstract Task ChangePlotVisibilityAsync(Plot plot, bool isVisible);

    public abstract Task ChangePlotSetVisibilityAsync(PlotSet set, bool isVisible);

    public abstract Task ChangePlotHighlightionAsync(Plot plot);

    public abstract Task ChangePlotSetHighlightionAsync(PlotSet set);

    public abstract void Clear();

    public abstract void Refresh();
}
