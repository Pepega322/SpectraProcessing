using Model.Command;

namespace Model.Data.Undefined;
public class Undefined : IData, IWriteable
{
    private readonly IEnumerable<string> _contents;
    public IData CreateCopy() => new Undefined(this);
    public IEnumerable<string> ToContents() => _contents;
    public void Edit(IDataEditCommand command) => throw new NotImplementedException();
    public IData GetInfo(IGetDataInfoCommand command) => throw new NotImplementedException();

    public Undefined(FileInfo file)
    {
        _contents = File.ReadAllLines(file.FullName);
    }

    private Undefined(Undefined reference)
    {
        _contents = reference._contents.ToArray();
    }
}
