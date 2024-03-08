using Controllers;

namespace View.Controllers;

public class WinformsDialogController : DialogController {
    public override string? SelectPathInDialog() {
        using FolderBrowserDialog dialog = new();
        DialogResult result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.SelectedPath : null;
    }

    public override string? SelectFullNameInDialog(string extension) {
        using SaveFileDialog dialog = new();
        dialog.DefaultExt = extension;
        DialogResult result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.FileName : null;
    }
}
