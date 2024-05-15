using Controllers;

namespace WinformsApp.Controllers;

public class WinformsDialogController : DialogController
{
	public override string? SelectPathInDialog()
	{
		using FolderBrowserDialog dialog = new();
		var result = dialog.ShowDialog();
		return result == DialogResult.OK ? dialog.SelectedPath : null;
	}

	public override string? SelectFullNameInDialog(string? defaultName, string? defaultExtension)
	{
		using SaveFileDialog dialog = new();
		dialog.FileName = defaultName ?? defaultName;
		dialog.DefaultExt = defaultExtension ?? defaultExtension;
		var result = dialog.ShowDialog();
		return result == DialogResult.OK ? dialog.FileName : null;
	}
}