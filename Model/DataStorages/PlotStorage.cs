using Model.DataFormats;

namespace Model.DataStorages;
internal class PlotStorage : DataStorage {
    public override bool Add(string setKey, DataSet set) {
        throw new NotImplementedException();
    }

    public override bool AddToDefaultSet(Data data) {
        throw new NotImplementedException();
    }

    protected override void AddDefaultSet() {
        throw new NotImplementedException();
    }
}
