using SpectraProcessing.Bll.Controllers.Interfaces;

namespace SpectraProcessing.Application.Controllers;

public class DialogController : IDialogController
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
        dialog.FileName = defaultName;
        dialog.DefaultExt = defaultExtension;
        var result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.FileName : null;
    }

    public string? GetReadFileFullName()
    {
        using OpenFileDialog dialog = new();
        var result = dialog.ShowDialog();
        return result == DialogResult.OK ? dialog.FileName : null;
    }
}
