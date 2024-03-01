using Model.DataFormats;

namespace Model.DataSources;

public class WindowsReader : DataReader {
    public override Data ReadData(string fullName) {
        var file = new FileInfo(fullName);
        if (!file.Exists)
            throw new FileNotFoundException(fullName);

        if (!Enum.TryParse(file.Extension.TrimStart('.'), true, out SpectraFormat format))
            return Data.Undefined;

        var contents = File.ReadAllLines(file.FullName);
        Data data;
        try {
            data = Spectra.Parse(format, file.Name, contents);
        }
        catch (Exception ex) {
            data = new Сorrupted(file.FullName, ex.Message);
        }
        return data;
    }
}
