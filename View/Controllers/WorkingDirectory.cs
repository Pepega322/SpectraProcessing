using Model.DataSources;
using Model.DataStorages;

namespace View.Controllers;
public class WorkingDirectory {
    private DirectoryInfo current;
    private DataSource source;
    public string FullName => current.FullName;

    public WorkingDirectory(string path) {
        current = new DirectoryInfo(path);
        source = new WindowsFileSystem();
    }

    public async Task ReadWithSubdirectoriesAsync(DataStorage storage) {
        var setKey = $"{current.Name} (all)";
        var rootSet = await Task.Run(() => new DirDataSet(setKey, source, current.FullName, true));
        storage.AddDataSet(setKey, rootSet);
    }

    public async Task ReadAsync(DataStorage storage) {
        var setKey = $"{current.Name} (only this)";
        var rootSet = await Task.Run(() => new DirDataSet(setKey, source, current.FullName, false));
        storage.AddDataSet(setKey, rootSet);
    }

    public void StepBack() {
        if (current.Parent != null)
            ChangeDirectory(current.Parent.FullName);
    }

    public void ChangeDirectory(string path) {
        current = new DirectoryInfo(path);
    }

    public IEnumerable<TreeNode> GetDirectoryTree() {
        foreach (var dir in current.GetDirectories())
            yield return new TreeNode { Text = dir.Name, Tag = dir, ImageIndex = 0 };
        foreach (var file in current.GetFiles())
            yield return new TreeNode { Text = file.Name, Tag = file, ImageIndex = 1, };
    }
}
