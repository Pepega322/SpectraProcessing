using Model.SupportedCommands.Base;
using Model.SupportedDataFormats.Base;

namespace Model.SupportedCommands.GetData.Base;
public abstract class GetDataCommand : Command
{
    public abstract Data Execute();
}
