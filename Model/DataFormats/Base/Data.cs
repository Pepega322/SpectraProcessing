namespace Model.DataFormats;
public abstract class Data {
    public static readonly Undefined Undefined = new("Undefined");

    public string Name { get; protected set; } = null!;

    protected Data(string name) {
        Name = name;
    }

    public override bool Equals(object? obj) {
        var data = obj as Data;
        if (data == null) return false;
        return Name.Equals(data.Name);
    }

    public override int GetHashCode() => Name.GetHashCode();
}
