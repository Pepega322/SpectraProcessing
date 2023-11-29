using Model.Data;

namespace Model.DataSource;
public interface IDataSourse
{
    IData ReadFile(string path);
    void WriteFile(IWriteable data, string path);
}
