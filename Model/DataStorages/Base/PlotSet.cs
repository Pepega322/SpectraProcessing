using Model.DataFormats;

namespace Model.DataStorages;
public abstract class PlotSet : DataSet, IEnumerable<Plot> {
    public PlotSet(string name)
        : base(name) { }

    protected override bool AddToSet(Data data) {
        if (data is not Plot) 
            throw new ArgumentException(nameof(data) + "isn't plot");
        bool result;
        lock (set) result = set.Add(data);
        return result;
    }

    IEnumerator<Plot> IEnumerable<Plot>.GetEnumerator() {
        foreach (var data in set)
            yield return (Plot)data;
    }
}
