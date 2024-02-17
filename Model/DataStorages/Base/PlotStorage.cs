
namespace Model.DataStorages;
public abstract class PlotStorage : DataStorage, IEnumerable<KeyValuePair<string, PlotSet>> {
    protected override bool AddSet(string setKey, DataSet set) {
        if (set is not PlotSet)
            throw new ArgumentException(nameof(set) + "isn't plot set");
        storage.Add(setKey, set);
        return true;
    }

    IEnumerator<KeyValuePair<string, PlotSet>> IEnumerable<KeyValuePair<string, PlotSet>>.GetEnumerator() {
        foreach (var pair in storage)
            yield return new KeyValuePair<string, PlotSet>(pair.Key, (PlotSet)pair.Value);

    }
}
