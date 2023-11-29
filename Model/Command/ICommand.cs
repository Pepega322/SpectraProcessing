using Model.Data;

namespace Model.Command;
public interface ICommand
{
    IData Execute(IData data);
}
