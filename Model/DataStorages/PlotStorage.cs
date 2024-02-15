using Model.DataFormats;

namespace Model.DataStorages;
public abstract class PlotStorage : Storage {
    protected Dictionary<IReadOnlyList<float>, float[]> xSPlots;

    public PlotStorage() : base() {
        xSPlots = new Dictionary<IReadOnlyList<float>, float[]>();
    }

    public override bool AddToDefaultSet(Data data) {
        if (data is not Plot) return false;
        return AddToDefaultSet(data);
    }


    public bool AddSet(string setKey, PlotSet set) {
        if (!storage.ContainsKey(setKey)) {
            storage.Add(setKey, set);
            return true;
        }
        return false;
    }
}
