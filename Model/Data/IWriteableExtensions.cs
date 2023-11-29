using Model.DataSource;

namespace Model.Data;
public static class IWriteableExtensions
{
    public static void WriteFile(this IWriteable data, IDataSourse sourse, string fullName)
    {
        sourse.WriteFile(data, fullName);
    }
}
