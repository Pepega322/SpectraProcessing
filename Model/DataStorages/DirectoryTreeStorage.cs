using Model.DataFormats;

namespace Model.DataStorages;
public class DirectoryTreeStorage : TreeDataStorage {
    public override bool Add(string setKey, DataSet set) {
        if (storage.ContainsKey(setKey)) return false;
        return AddSet(setKey, set);
    }

    public override bool AddToDefaultSet(Data data) {
        if (data is not Spectra) return false;
        return DefaultSet.Add(data);
    }

    protected override void AddDefaultSet() {
        storage.Add(DefaultSetKey, new DirectorySetNode(DefaultSetKey));
    }
}
