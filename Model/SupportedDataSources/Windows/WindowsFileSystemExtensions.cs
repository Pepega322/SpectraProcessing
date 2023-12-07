using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Interfaces;

namespace Model.SupportedDataSources.Windows;
public static class WindowsFileSystemExtensions
{
    public static async Task<Data> ReadFileAsync(this WindowsFileSystem sys, string fullName)
        => await Task.Run(() => sys.ReadFile(fullName));

    public static async Task<Data> ReadFileAsync(this WindowsFileSystem sys, FileInfo fileInfo)
        => await sys.ReadFileAsync(fileInfo.FullName);

    public static Data[] ReadFilesAtDirectory(this WindowsFileSystem sys, DirectoryInfo directory)
    {
        var result = directory.GetFiles().Select(sys.ReadFileAsync).ToArray();
        Task.WaitAll(result);
        return result.Select(t => t.Result).ToArray();
    }

    public static async Task<Data[]> ReadFilesAtDirectoryAsync(this WindowsFileSystem sys, DirectoryInfo directory)
        => await Task.Run(() => ReadFilesAtDirectory(sys, directory));

    public static async Task<Data[]> ReadFilesAtDirectoryAsync(this WindowsFileSystem sys, string fullPath)
        => await sys.ReadFilesAtDirectoryAsync(new DirectoryInfo(fullPath));

    public static async Task WriteFileAsync(this WindowsFileSystem sys, IWriteable data, string fullName)
        => await Task.Run(() => sys.WriteFile(data, fullName));

    public static async Task WriteFileAsync(this WindowsFileSystem sys, IWriteable data, DirectoryInfo directory, string nameDotExtension)
        => await sys.WriteFileAsync(data, Path.Combine(directory.FullName, nameDotExtension));

    public static async Task WriteFileAsync(this WindowsFileSystem sys, IWriteable data, DirectoryInfo directory, string name, string dotExtension)
        => await sys.WriteFileAsync(data, directory, name + dotExtension);
}
