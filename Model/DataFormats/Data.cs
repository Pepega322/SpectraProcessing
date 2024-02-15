namespace Model.DataFormats;
public abstract class Data : IComparable {
    public static readonly Undefined Undefined = new("Undefined");

    public string Name { get; protected set; } = null!;

    public static Data Convert(DataFormat format, string contentsSourceId, string dataName, string[] contents) {
        Data data;
        try {
            data = format switch {
                DataFormat.ASP => new ASP(dataName, contents),
                DataFormat.ESP => new ESP(dataName, contents),
                _ => throw new NotSupportedException()
            };
        }
        catch (IndexOutOfRangeException ex) {
            data = new Сorrupted(contentsSourceId, ex.Message);
        }
        catch (FormatException ex) {
            data = new Сorrupted(contentsSourceId, ex.Message);
        }
        return data;
    }

    public int CompareTo(object? obj) {
        if (obj == null) return 1;
        if (obj is not Data data)
            throw new ArgumentException("Object is not Data");
        return CompareTo(data);
    }

    protected virtual int CompareTo(Data data) => Name.CompareTo(data.Name);
}
