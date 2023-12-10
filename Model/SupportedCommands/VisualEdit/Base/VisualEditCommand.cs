using Model.SupportedCommands.Base;

namespace Model.SupportedCommands.VisualEdit.Base;
public abstract class VisualEditCommand : Command
{
    public abstract void Execute();
}
