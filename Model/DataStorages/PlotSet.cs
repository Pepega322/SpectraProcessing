using Model.DataFormats;

namespace Model.DataStorages;
internal class PlotSet : DataSet {
    public PlotSet(string name)
        : base(name) {
    }

    public override bool Add(Data data) {
        throw new NotImplementedException();
    }

    public override bool Remove(Data data) {
        throw new NotImplementedException();
    }
}
