using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Interfaces;
using Model.SupportedDataFormats.SupportedSpectraFormats;
using Model.SupportedDataFormats.Unsupported;
using Model.SupportedDataSources.Base;

namespace Model.SupportedDataSources.Windows;

public class WindowsFileSystem : DataSource
{
    public override Data ReadFile(string fullName)
    {
        var file = new FileInfo(fullName);
        var contents = File.ReadAllLines(file.FullName);
        if (contents.Length == 0)
            return new Empty(file.FullName);

        return file.Extension switch
        {
            ".asp" => new ASP(file.Name, contents),
            ".esp" => new ESP(file.Name, contents),
            _ => new Undefined(file.Name, [file.FullName])
        };
    }

    public override void WriteFile(IWriteable data, string fullName)
    {
        var file = File.Create(fullName);
        file.Close();
        File.AppendAllLines(fullName, data.ToContents());
    }

    //public void WriteFile(string[] contents, string fullName)
    //{
    //    var file = File.Create(fullName);
    //    file.Close();
    //    File.AppendAllLines(fullName, contents);
    //}
}
