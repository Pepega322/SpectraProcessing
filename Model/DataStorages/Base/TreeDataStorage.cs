namespace Model.DataStorages;
public abstract class TreeDataStorage : DataStorage {
    protected override bool AddSet(string setKey, DataSet set) {
        if (set is not TreeDataSetNode) return false;
        storage.Add(setKey, set);
        return true;
    }
}
