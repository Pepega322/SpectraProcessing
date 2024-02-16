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
        if (Spectra.TryParse(format, file.Name, contents, out Spectra? spectra, out string? message))
            return spectra;

        return new Сorrupted(file.FullName, message);
    }
}
