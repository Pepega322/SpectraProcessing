using Model.SupportedDataFormats.Base;

namespace Model.SupportedCommands.Base;
public abstract class Command
{
    public abstract Data Execute(Data data);
}
