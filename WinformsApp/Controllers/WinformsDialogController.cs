using Controllers;

namespace View.Controllers;

public class WinformsDialogController : DialogController {
    public override string? SelectPathInDialog() {
        using FolderBrowserDialog dialog = new();
        DialogResult result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.SelectedPath : null;
    }

    public override string? SelectFullNameInDialog(string? defaultName = null, string? defaultExtension = null) {
        using SaveFileDialog dialog = new();
        dialog.FileName = defaultName ?? defaultName;
        dialog.DefaultExt = defaultExtension ?? defaultExtension;
        DialogResult result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.FileName : null;
    }
}
