using Model.Command;

namespace Model.Data;
public interface IData
{
    void Edit(IDataEditCommand command);
    IData GetInfo(IGetDataInfoCommand command);
    IData CreateCopy();
}
