using Model.DataFormats;
using Model.DataStorages;

namespace View.Controllers;
public abstract class PlotController {
    protected DataStorage storage;
    public DataSet ContextSet { get; private set; } = null!;
    public Plot ContextPlot { get; private set; } = null!;

    protected PlotController(DataStorage storage) {
        this.storage = storage;
        ContextSet = storage.DefaultSet;
    }

    public abstract Task PlotData(Data data);

    public abstract Task PlotSet(TreeDataSetNode set);

    public abstract Task RemoveData(Data data);

    public abstract Task RemoveSet(TreeDataSetNode set);

    public abstract Task Clear();

    public abstract IEnumerable<TreeNode> GetTree();
}
