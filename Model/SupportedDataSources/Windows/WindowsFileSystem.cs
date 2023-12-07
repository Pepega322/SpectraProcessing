using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Interfaces;
using Model.SupportedDataFormats.SupportedSpectraFormats;
using Model.SupportedDataFormats.Undefined;
using Model.SupportedDataSources.Base;

namespace Model.SupportedDataSources.Windows;

public class WindowsFileSystem : DataSourse
{
    public override Data ReadFile(string fullName)
    {
        var file = new FileInfo(fullName);
        if (!file.Exists)
            throw new FileNotFoundException(fullName);
        return file.Extension switch
        {
            ".asp" => new ASP(file.Name, File.ReadAllLines(file.FullName)),
            ".esp" => new ESP(file.Name, File.ReadAllLines(file.FullName)),
            _ => new Undefined(file.Name, File.ReadAllLines(file.FullName))
        };
    }

    public override void WriteFile(IWriteable data, string fullName)
    {
        var file = File.Create(fullName);
        file.Close();
        File.AppendAllLines(fullName, data.ToContents());
    }
}
