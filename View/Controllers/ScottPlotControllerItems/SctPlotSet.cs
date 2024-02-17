using Model.DataFormats;
using Model.DataStorages;

namespace View.Controllers;
internal class SctPlotSet : PlotSet, IEnumerable<SctPlot> {
    public SctPlotSet(string name)
        : base(name) {
    }

    public override bool Add(Data data) {
        if (data is not SctPlot)
            throw new ArgumentException(nameof(data) + "isn't SctPlot");
        return AddToSet(data);
    }

    public override bool Remove(Data data) => RemoveFromSet(data);

    IEnumerator<SctPlot> IEnumerable<SctPlot>.GetEnumerator() {
        foreach (var data in set)
            yield return (SctPlot)data;
    }
}
