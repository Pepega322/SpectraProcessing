using Model.DataFormats;
using Model.DataStorages;
using ScottPlot;

namespace View.Controllers;
public abstract class PlotController {
    protected Dictionary<IReadOnlyList<float>, float[]> xSPlots;
    protected SortedDictionary<Data, IPlottable> plots;

    protected PlotController() {
        xSPlots = new Dictionary<IReadOnlyList<float>, float[]>();
        plots = new SortedDictionary<Data, IPlottable>();
    }

    public abstract Task PlotSet(DataSetNode set);

    public abstract Task PlotData(Data data);

    public abstract Task RemoveSet(DataSetNode set);

    public abstract Task RemoveData(DataSetNode set);

    public abstract Task Clear();

    public abstract IEnumerable<TreeNode> GetTree();
}
