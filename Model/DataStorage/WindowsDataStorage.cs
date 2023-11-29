using Model.Data;

namespace Model.DataStorage;
internal class WindowsDataStorage : IDataStorage
{
    public void Add(string id, IData data) => throw new NotImplementedException();
    public IData Get(string id) => throw new NotImplementedException();
    public void Remove(string id) => throw new NotImplementedException();
}
