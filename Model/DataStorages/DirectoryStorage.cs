namespace Model.DataStorages;
public class DirectoryStorage : DataStorage {
    protected override void AddDefaultSet() {
        storage.Add(DefaultSetKey, new DirectoryDataSet(DefaultSetKey));
    }
}
