namespace Controllers.Interfaces;

public interface IDialogController
{
	string? SelectPathInDialog();
	string? SelectFullNameInDialog(string defaultName, string defaultExtension);
}