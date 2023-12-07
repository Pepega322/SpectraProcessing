using Model.SupportedCommands.VisualEdit.Base;
using Model.SupportedDataFormats.Base;

namespace Model.SupportedVisualFormats.Base;
public abstract class Visual
{
    public string Name { get; protected set; } = null!;
    protected readonly Data _source = null!;

    public abstract void Edit(VisualEditCommand command);

    public abstract Visual CreateCopy();
}
