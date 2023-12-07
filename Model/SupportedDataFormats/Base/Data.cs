using Model.SupportedCommands.DataEdit.Base;
using Model.SupportedCommands.GetData.Base;

namespace Model.SupportedDataFormats.Base;
public abstract class Data
{
    public string Name { get; protected set; } = null!;

    public abstract Data CreateCopy();
    public abstract void Edit(DataEditCommand command);
    public abstract Data GetInfo(GetDataCommand command);
}
