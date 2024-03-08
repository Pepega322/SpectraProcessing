namespace Domain.DataSource;
public interface IWriteable {
    string Name { get; set; }

    IEnumerable<string> ToContents();
}
