using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Interfaces;

namespace Model.SupportedDataSources.Base;
public abstract class DataSourse
{
    public abstract Data ReadFile(string path);
    public abstract void WriteFile(IWriteable data, string path);
}
