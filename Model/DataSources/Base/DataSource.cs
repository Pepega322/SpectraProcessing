using Model.DataFormats.Base;
using Model.DataFormats.Interfaces;

namespace Model.DataSources.Base;
public abstract class DataSource
{
    public abstract Data ReadData(string path);
    public abstract void WriteData(IWriteable data, string path);
}
