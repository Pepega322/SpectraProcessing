using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;

namespace View.Controllers;
public abstract class DataController  {
    protected DataWriter writer;
    protected TreeDataStorage storage;
    public TreeDataSetNode ContextSet { get; private set; } = null!;
    public Data ContextData { get; private set; } = null!;

    public DataController(DataWriter writer, TreeDataStorage storage) {
        this.writer = writer;
        this.storage = storage;
        ContextSet = (TreeDataSetNode)storage.DefaultSet;
    }

    public abstract Task WriteDataAsAsync(string path, string extension);

    public abstract Task WriteSetAsAsync(string path, string extension, bool writeSubsets);

    public bool AddSet(string setKey, TreeDataSetNode setNode) => storage.Add(setKey, setNode);

    public bool AddDataToDefaultSet(Data data) => storage.AddToDefaultSet(data);

    public bool DeleteSet() {
        var isSetInStorageRoot = storage.ContainsSet(ContextSet.Name) && ContextSet != storage.DefaultSet;
        return (isSetInStorageRoot) ? storage.Remove(ContextSet.Name) : ContextSet.DisconnectFromParent();
    }

    public bool DeleteData() => ContextSet.Remove(ContextData);

    public bool ChangeContextSet(TreeDataSetNode set) {
        ContextSet = set;
        return true;
    }

    public bool ChangeContextData(Data data) {
        ContextData = data;
        return true;
    }

    public void Clear() {
        storage.Clear();
    }

    public abstract IEnumerable<TreeNode> GetTree();
}
