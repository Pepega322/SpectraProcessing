using Model.SupportedCommands.DataEdit.Base;
using Model.SupportedCommands.GetData.Base;
using Model.SupportedDataFormats.Base;
using Model.SupportedDataFormats.Interfaces;

namespace Model.SupportedDataFormats.Undefined;
public class Undefined : Data, IWriteable
{
    private readonly IEnumerable<string> _contents;

    public Undefined(string name, string[] contents)
    {
        Name = name;
        _contents = contents;
    }

    private Undefined(Undefined reference)
    {
        Name = reference.Name;
        _contents = reference._contents.ToArray();
    }

    public override Data CreateCopy() => new Undefined(this);
    public override void Edit(DataEditCommand command) => throw new NotImplementedException();
    public override Data GetInfo(GetDataCommand command) => throw new NotImplementedException();
    public IEnumerable<string> ToContents() => _contents;
}
