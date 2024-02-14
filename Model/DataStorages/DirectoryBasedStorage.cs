using Model.DataSources;

namespace Model.DataStorages;
public class DirectoryBasedStorage : DataStorage {
    public DirectoryBasedStorage(DataSource source, string name)
        : base(source, name) { }

    protected override void AddDefaultSet() {
        storage.Add(DefaultDataSetKey, new DirDataSet(DefaultDataSetKey));
    }
}
