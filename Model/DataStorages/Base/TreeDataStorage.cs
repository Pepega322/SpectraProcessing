namespace Model.DataStorages;
public abstract class TreeDataStorage : DataStorage, IEnumerable<KeyValuePair<string, TreeDataSetNode>> {
    protected override bool AddSet(string setKey, DataSet set) {
        if (set is not TreeDataSetNode) 
            throw new ArgumentException(nameof(set) + "isn't treeData set");
        storage.Add(setKey, set);
        return true;
    }

    IEnumerator<KeyValuePair<string, TreeDataSetNode>> IEnumerable<KeyValuePair<string, TreeDataSetNode>>.GetEnumerator() {
        foreach (var pair in storage)
            yield return new KeyValuePair<string, TreeDataSetNode>(pair.Key, (TreeDataSetNode)pair.Value);
    }
}
