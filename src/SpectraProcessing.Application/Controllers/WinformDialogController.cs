using SpectraProcessing.Bll.Controllers.Interfaces;

namespace SpectraProcessing.Application.Controllers;

internal sealed class WinformDialogController : IDialogController
{
    public string? GetFolderPath()
    {
        using FolderBrowserDialog dialog = new();
        var result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.SelectedPath : null;
    }

    public string? GetSaveFileFullName(string defaultName, string defaultExtension)
    {
        using SaveFileDialog dialog = new();
        dialog.FileName = $"{defaultName}.{defaultExtension}";

        if (dialog.ShowDialog() is not DialogResult.OK)
        {
            return null;
        }

        return dialog.FileName.EndsWith($".{defaultExtension}")
            ? dialog.FileName
            : $"{dialog.FileName}.{defaultExtension}";
    }

    public string? GetReadFileFullName()
    {
        using OpenFileDialog dialog = new();
        var result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.FileName : null;
    }
}
