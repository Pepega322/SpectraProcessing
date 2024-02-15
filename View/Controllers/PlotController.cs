using Model.DataFormats;
using Model.DataStorages;

namespace View.Controllers;
public abstract class PlotController {
    protected PlotStorage storage;

    protected PlotController(PlotStorage storage) {
        this.storage = storage;
    }

    public abstract Task PlotSet(PlotSet set);

    public abstract Task PlotData(Data data);

    public abstract Task RemoveSet(PlotSet set);

    public abstract Task RemoveData(Data set);

    public abstract Task Clear();

    public abstract IEnumerable<TreeNode> GetTree();
}
