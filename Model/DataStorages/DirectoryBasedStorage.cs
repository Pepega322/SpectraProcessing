using Model.DataSources.Base;
using Model.DataStorages.Base;

namespace Model.DataStorages;
public class DirectoryBasedStorage : DataStorage
{
    public DirectoryBasedStorage(DataSource source, string name)
        : base(source, name) { }

    protected override void AddDefaultSet()
    {
        _storage.Add(DefaultDataSetKey, new DirDataSet(DefaultDataSetKey));
    }
}
