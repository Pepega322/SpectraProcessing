using Model.Command;

namespace Model.Visual;
public interface IVisual
{
    void Edit(IVisualEditCommand command);
    IVisual CreateCopy();
}
