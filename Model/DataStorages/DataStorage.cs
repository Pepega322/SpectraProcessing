using Model.DataFormats;

namespace Model.DataStorages;
public abstract class DataStorage : Storage {
    public bool AddSet(string setKey, TreeSet set) {
        if (!storage.ContainsKey(setKey)) {
            storage.Add(setKey, set);
            return true;
        }
        return false;
    }

    public override bool AddToDefaultSet(Data data) {
        if (data is Undefined or Сorrupted or Plot) return false;
        return DefaultSet.Add(data);
    }
}
