using Model.DataFormats;

namespace Model.DataSources;
public class WindowsWriter : DataWriter {
    public override void WriteData(IWriteable data, string fullName) {
        var file = File.Create(fullName);
        file.Close();
        File.AppendAllLines(fullName, data.ToContents());
    }
}