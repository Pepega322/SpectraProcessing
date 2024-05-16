using Controllers.Interfaces;

namespace Application.Controllers;

public class WinformsDialogController : IDialogController
{
	public string? SelectPathInDialog()
	{
		using FolderBrowserDialog dialog = new();
		var result = dialog.ShowDialog();
		return result == DialogResult.OK ? dialog.SelectedPath : null;
	}

	public string? SelectFullNameInDialog(string? defaultName, string? defaultExtension)
	{
		using SaveFileDialog dialog = new();
		dialog.FileName = defaultName ?? defaultName;
		dialog.DefaultExt = defaultExtension ?? defaultExtension;
		var result = dialog.ShowDialog();
		return result == DialogResult.OK ? dialog.FileName : null;
	}
}