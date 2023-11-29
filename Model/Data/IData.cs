using Model.Command;

namespace Model.Data;
internal interface IData
{
    void Edit(IDataEditCommand command);
    IData GetInfo(IGetDataInfoCommand command);
    IData CreateCopy();
}
