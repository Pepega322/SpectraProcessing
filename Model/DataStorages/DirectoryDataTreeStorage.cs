using Model.DataFormats;

namespace Model.DataStorages;
public class DirectoryDataTreeStorage : TreeDataStorage, IEnumerable<KeyValuePair<string, DirectoryDataSetNode>> {
    public override bool Add(string setKey, DataSet set) {
        string key;
        if (storage.ContainsKey(setKey)) 
            key = GetNewSetKey(setKey);
        else key = setKey;
        return AddSet(key, set);
    }

    public override bool AddToDefaultSet(Data data) {
        if (data is not Spectra) return false;
        return DefaultSet.Add(data);
    }

    protected override void AddDefaultSet() {
        AddSet(DefaultSetKey, new DirectoryDataSetNode(DefaultSetKey));
    }

    IEnumerator<KeyValuePair<string, DirectoryDataSetNode>> IEnumerable<KeyValuePair<string, DirectoryDataSetNode>>.GetEnumerator() {
        foreach (var pair in storage)
            yield return new KeyValuePair<string, DirectoryDataSetNode>(pair.Key, (DirectoryDataSetNode)pair.Value);
    }
}
