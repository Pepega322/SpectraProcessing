using Model.DataFormats;

namespace Model.DataSources;
public abstract class DataReader {
    public abstract Data ReadData(string path);

}
