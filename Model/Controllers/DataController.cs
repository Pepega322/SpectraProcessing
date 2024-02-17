using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;

namespace Model.Controllers;
public abstract class DataController {
    protected DataWriter writer;
    protected DataStorage storage;

    public DataController(DataWriter writer, DataStorage storage) {
        this.writer = writer;
        this.storage = storage;
    }

    public abstract Task WriteDataAsAsync(Data data, string path, string extension);

    public abstract Task WriteSetAsAsync(DataSet set, string path, string extension, bool writeSubsets);

    public bool AddSet(string setKey, DataSet setNode) => storage.Add(setKey, setNode);

    public bool AddDataToDefaultSet(Data data) => storage.AddToDefaultSet(data);

    public bool DeleteSet(DataSet set) {
        if (storage.Remove(set.Name)) return true;
        if (set is TreeDataSetNode node) {
            node.DisconnectFromParent();
            return true;
        }
        throw new NotImplementedException("DataController delete set problem");
    }

    public bool DeleteData(DataSet dataOwner, Data data) => dataOwner.Remove(data);

    public void Clear() {
        storage.Clear();
    }
}
