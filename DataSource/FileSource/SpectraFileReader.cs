using Domain.DataSource;
using Domain.SpectraData;
using Domain.SpectraData.Support;

namespace DataSource.FileSource;
public class SpectraFileReader(ISpectraParser parser) : IDataSource<Spectra> {
    public Spectra Get(string fullName) {
        var file = new FileInfo(fullName);
        if (!file.Exists)
            throw new FileNotFoundException(fullName);

        if (!Enum.TryParse(file.Extension.TrimStart('.'), true, out SpectraFormat format))
            throw new UndefinedFileException(fullName);

        var contents = File.ReadAllLines(file.FullName);
        Spectra data;
        try {
            data = parser.Parse(format, file.Name, contents);
        }
        catch (Exception e) {
            throw new CorruptedFileException($"{fullName}{Environment.NewLine}{e.Message}");
        }
        return data;
    }
}
