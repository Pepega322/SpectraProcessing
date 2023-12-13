using Model.DataFormats.Base;

namespace Model.DataFormats.Unsupported;
public class Сorrupted : Data
{
    public readonly string ExceptionMessage;

    internal Сorrupted(string fullName, string exceptionMessage)
    {
        Name = fullName;
        ExceptionMessage = exceptionMessage;
    }
}
