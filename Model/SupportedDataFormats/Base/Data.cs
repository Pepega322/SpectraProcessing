using Model.SupportedCommands.DataEdit.Base;
using Model.SupportedCommands.GetData.Base;

namespace Model.SupportedDataFormats.Base;
public abstract class Data : IComparable
{
    public string Name { get; protected set; } = null!;

    public abstract Data CreateCopy();
    public abstract void Edit(DataEditCommand command);
    public abstract Data GetInfo(GetDataCommand command);
    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;
        if (obj is not Data data)
            throw new ArgumentException("Object is not Data");
        return CompareTo(data);
    }

    protected virtual int CompareTo(Data data) => Name.CompareTo(data.Name);
}
