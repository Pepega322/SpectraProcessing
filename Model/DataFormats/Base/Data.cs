namespace Model.DataFormats;
public abstract class Data {
    public static readonly Undefined Undefined = new("Undefined");

    public string Name { get; protected set; } = null!;

    protected Data(string name) {
        Name = name;
    }
}
