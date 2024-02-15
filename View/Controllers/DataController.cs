using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;

namespace View.Controllers;
public abstract class DataController  {
    protected DataWriter writer;
    protected DataStorage storage;
    public DataSetNode ContextSet { get; private set; } = null!;
    public Data ContextData { get; private set; } = null!;

    public DataController(DataWriter writer, DataStorage storage) {
        this.writer = writer;
        this.storage = storage;
        ContextSet = storage.DefaultDataSet;
    }

    public abstract Task WriteDataAsAsync(string path, string extension);

    public abstract Task WriteSetAsAsync(string path, string extension, bool writeSubsets);

    public bool AddSet(string setKey, DataSetNode setNode) => storage.AddDataSet(setKey, setNode);

    public bool AddDataToDefaultSet(Data data) => storage.AddToDefaultSet(data);

    public bool DeleteSet() {
        var isSetInStorageRoot = storage.ContainsSet(ContextSet.Name) && ContextSet != storage.DefaultDataSet;
        return (isSetInStorageRoot) ? storage.RemoveSet(ContextSet.Name) : ContextSet.DisconnectFromParent();
    }

    public bool DeleteData() => ContextSet.Remove(ContextData);

    public bool ChangeContextSet(DataSetNode set) {
        ContextSet = set;
        return true;
    }

    public bool ChangeContextData(Data data) {
        ContextData = data;
        return true;
    }

    public abstract IEnumerable<TreeNode> GetTree();
}
