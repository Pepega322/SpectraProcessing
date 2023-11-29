using Model.Data;

namespace Model.DataStorage;
internal interface IDataStorage
{
    void Add(IData data);
    IData Get(string dataId);
    void Remove(string dataId);
}
