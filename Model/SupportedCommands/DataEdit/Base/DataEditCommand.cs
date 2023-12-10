using Model.SupportedCommands.Base;

namespace Model.SupportedCommands.DataEdit.Base;
public abstract class DataEditCommand : Command
{
    public abstract void Execute();
}
