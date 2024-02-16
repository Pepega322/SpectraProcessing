namespace Model.DataFormats;
public class Сorrupted : Data {
    public readonly string ExceptionMessage;

    public Сorrupted(string fullName, string exceptionMessage)
        : base(fullName) {
        ExceptionMessage = exceptionMessage;
    }
}
