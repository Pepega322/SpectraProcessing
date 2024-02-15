using Model.DataFormats;

namespace Model.DataSources;
public class WindowsFileSystem : DataSource {
    public WindowsFileSystem() : base(new WindowsReader(), new WindowsWriter()) {
    }
}

public class WindowsReader : DataReader {
    public override Data ReadData(string fullName) {
        var file = new FileInfo(fullName);
        if (!file.Exists)
            throw new FileNotFoundException(fullName);

        if (!Enum.TryParse(file.Extension.TrimStart('.'), true, out DataFormat format))
            return Data.Undefined;

        var contents = File.ReadAllLines(file.FullName);
        var data = Data.Convert(format, fullName, file.Name, contents);

        return data;
    }
}

public class WindowsWriter : DataWriter {
    public override void WriteData(IWriteable data, string fullName) {
        var file = File.Create(fullName);
        file.Close();
        File.AppendAllLines(fullName, data.ToContents());
    }
}