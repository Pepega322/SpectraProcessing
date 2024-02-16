using Model.DataFormats;

namespace Model.DataSources;
public abstract class DataWriter {
    public abstract void WriteData(IWriteable data, string path);

}
