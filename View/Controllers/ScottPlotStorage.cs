using Model.DataStorages;

namespace View.Controllers;
internal class ScottPlotStorage : PlotStorage {
    protected override void AddDefaultSet() {
        storage.Add(DefaultSetKey, new ScottPlotSet(DefaultSetKey));
    }
}

public class ScottPlotSet : PlotSet {
    public ScottPlotSet(string name) : base(name) {
    }
}
