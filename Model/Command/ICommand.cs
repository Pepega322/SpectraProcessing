using Model.Data;

namespace Model.Command;
internal interface ICommand
{
    IData Execute();
}
