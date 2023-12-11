using Model.SupportedCommands.DataEdit.Base;
using Model.SupportedCommands.GetData.Base;
using Model.SupportedDataFormats.Base;

namespace Model.SupportedDataFormats.Unsupported;
public class Empty : Data
{
    public Empty(string fullName)
    {
        Name = fullName;
    }

    public override Data CreateCopy() => new Empty(Name);
    public override void Edit(DataEditCommand command) => throw new NotImplementedException();
    public override Data GetInfo(GetDataCommand command) => throw new NotImplementedException();
}
