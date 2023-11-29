using Model.Data;

namespace Model.DataStorage;
public interface IDataStorage
{
    void Add(string id, IData data);
    IData Get(string id);
    void Remove(string id);
}
