using Model.DataFormats;
using Model.DataStorages;

namespace View.Controllers;
internal class SctPlotStorage : PlotStorage, IEnumerable<KeyValuePair<string, SctPlotSet>> {
    public override bool Add(string setKey, DataSet set) {
        if (set is not SctPlotSet)
            throw new ArgumentException(nameof(set) + "isn't SctPlot set");
        return AddSet(setKey, set);
    }

    public override bool AddToDefaultSet(Data data) {
        if (data is not SctPlot)
            throw new ArgumentException(nameof(data) + "isn't SctPlot");
        return DefaultSet.Add(data);
    }

    protected override void AddDefaultSet() {
        AddSet(DefaultSetKey, new SctPlotSet(DefaultSetKey));
    }

    IEnumerator<KeyValuePair<string, SctPlotSet>> IEnumerable<KeyValuePair<string, SctPlotSet>>.GetEnumerator() {
        foreach (var pair in storage)
            yield return new KeyValuePair<string, SctPlotSet>(pair.Key, (SctPlotSet)pair.Value);
    }
}
