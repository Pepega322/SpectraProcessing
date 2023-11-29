using Model.Data;

namespace Model.DataSource;
internal interface IDataSourse
{
    IData ReadFile(string path);
    void WriteFile(string path, IData data);
}
