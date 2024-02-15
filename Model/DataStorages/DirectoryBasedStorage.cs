namespace Model.DataStorages;
public class DirectoryBasedStorage : DataStorage {
    public DirectoryBasedStorage(string name = "Storage")
        : base(name) { }

    protected override void AddDefaultSet() {
        storage.Add(DefaultDataSetKey, new DirDataSet(DefaultDataSetKey));
    }
}
