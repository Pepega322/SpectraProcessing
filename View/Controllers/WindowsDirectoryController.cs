using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;

namespace View.Controllers;
public class WindowsDirectoryController : RootController {
    private DirectoryInfo current;

    public WindowsDirectoryController(string path) : base(new WindowsReader()) {
        current = new DirectoryInfo(path);
    }

    public override async Task<TreeDataSetNode> ReadRoot(bool readSubdirs = false)
        => await Task.Run(() => new DirectorySetNode(current.Name, reader, current.FullName, readSubdirs));

    public override async Task<Data> ReadData(string fullName)
        => await Task.Run(() => reader.ReadData(fullName));

    public override bool StepBack()
        => current.Parent != null ? ChangeRoot(current.Parent.FullName) : false;

    public override bool ChangeRoot(string path) {
        var newDir = new DirectoryInfo(path);
        if (newDir.Exists) {
            current = new DirectoryInfo(path);
            return true;
        }
        return false;
    }

    public override IEnumerable<TreeNode> GetTree() {
        foreach (var dir in current.GetDirectories())
            yield return new TreeNode { Text = dir.Name, Tag = dir, ImageIndex = 0 };
        foreach (var file in current.GetFiles())
            yield return new TreeNode { Text = file.Name, Tag = file, ImageIndex = 1, };
    }

    public override string? SelectPathInDialog() {
        using (FolderBrowserDialog dialog = new()) {
            dialog.SelectedPath = current.FullName;
            DialogResult result = dialog.ShowDialog();
            return result == DialogResult.OK ? dialog.SelectedPath : null;
        }
    }
}
