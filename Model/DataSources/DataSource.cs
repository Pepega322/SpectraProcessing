using Model.DataFormats;

namespace Model.DataSources;
public abstract class DataSource {
    protected DataReader reader;
    protected DataWriter writer;

    public DataSource(DataReader reader, DataWriter writer) {
        this.reader = reader;
        this.writer = writer;
    }

    public virtual Data ReadData(string path) => reader.ReadData(path);
    public virtual void WriteData(IWriteable data, string path) => writer.WriteData(data, path);
}
