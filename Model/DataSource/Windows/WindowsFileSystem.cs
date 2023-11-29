using Model.Data;
using Model.Data.Spectra;
using Model.Data.Undefined;

namespace Model.DataSource.Windows;
public class WindowsFileSystem : IDataSourse
{
    public IData ReadFile(string fullName)
    {
        var file = new FileInfo(fullName);
        if (!file.Exists)
            throw new FileNotFoundException(file.FullName);
        return file.Extension switch
        {
            ".asp" => new ASPSpectra(file),
            ".esp" => new ESPSpectra(file),
            _ => new Undefined(file)
        };
    }

    public IData ReadFile(FileInfo fileInfo)
        => ReadFile(fileInfo.FullName);

    public IEnumerable<IData> ReadFilesAtDirectory(DirectoryInfo directory)
    {
        if (!directory.Exists)
            throw new DirectoryNotFoundException(directory.FullName);
        foreach (FileInfo file in directory.GetFiles())
            yield return ReadFile(file);
    }

    public IEnumerable<IData> ReadFilesAtDirectory(string fullPath)
        => ReadFilesAtDirectory(new DirectoryInfo(fullPath));

    public IEnumerable<IData> ReadFilesAtAllDirectories(DirectoryInfo rootDirectory)
    {
        if (!rootDirectory.Exists)
            throw new DirectoryNotFoundException(rootDirectory.FullName);
        var queue = new Queue<DirectoryInfo>();
        queue.Enqueue(rootDirectory);
        while (queue.Count != 0)
        {
            DirectoryInfo dir = queue.Dequeue();
            foreach (FileInfo file in dir.GetFiles())
                yield return ReadFile(file);
            foreach (DirectoryInfo nextDir in dir.GetDirectories())
                queue.Enqueue(nextDir);
        }
    }

    public IEnumerable<IData> ReadFilesAtAllDirectories(string fullPathToRoot)
        => ReadFilesAtAllDirectories(new DirectoryInfo(fullPathToRoot));

    public void WriteFile(IWriteable data, string fullName)
    {
        FileStream file = File.Create(fullName);
        file.Close();
        File.AppendAllLines(fullName, data.ToContents());
    }

    public void WriteFile(IWriteable data, DirectoryInfo directory, string nameDotExtension)
    {
        string fullName = Path.Combine(directory.FullName, nameDotExtension);
        WriteFile(data, fullName);
    }

    public void WriteFile(IWriteable data, DirectoryInfo directory, string name, string dotExtension)
    {
        string nameWithExtension = name + dotExtension;
        WriteFile(data, directory, nameWithExtension);
    }
}
