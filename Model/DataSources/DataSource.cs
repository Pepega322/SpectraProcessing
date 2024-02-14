using Model.DataFormats;

namespace Model.DataSources;
public abstract class DataSource
{
    public abstract Data ReadData(string path);
    public abstract void WriteData(IWriteable data, string path);
}
