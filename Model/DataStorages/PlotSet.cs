using Model.DataFormats;

namespace Model.DataStorages;
public abstract class PlotSet : Set {
    public PlotSet(string name)
        : base(name) { }

    public override bool Add(Data data) {
        if (data is not Plot) return false;
        bool result;
        lock (set) result = set.Add(data);
        return result;
    }

    public override bool Remove(Data data) {
        bool result;
        lock (set) result = set.Remove(data);
        return result;
    }
}
