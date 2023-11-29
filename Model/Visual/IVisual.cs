using Model.Command;

namespace Model.Visual;
internal interface IVisual
{
    void Edit(IVisualEditCommand command);
    IVisual CreateCopy();
}
