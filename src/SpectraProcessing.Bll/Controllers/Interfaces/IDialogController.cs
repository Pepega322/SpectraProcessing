namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface IDialogController
{
    string? GetFolderPath();

    string? GetSaveFileFullName(string defaultName, string defaultExtension);

    string? GetReadFileFullName();
}
