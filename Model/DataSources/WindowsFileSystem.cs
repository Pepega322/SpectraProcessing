using Model.DataFormats;

namespace Model.DataSources;
public class WindowsFileSystem : DataSource {
    public override Data ReadData(string fullName) {
        var file = new FileInfo(fullName);
        if (!file.Exists)
            throw new FileNotFoundException(fullName);

        if (!Enum.TryParse(file.Extension.TrimStart('.'), true, out DataFormat format))
            return Data.Undefined;

        var contents = File.ReadAllLines(file.FullName);
        var data = Data.ContentsToData(format, fullName, file.Name, contents);

        return data;
    }

    public override void WriteData(IWriteable data, string fullName) {
        var file = File.Create(fullName);
        file.Close();
        File.AppendAllLines(fullName, data.ToContents());
    }
}
