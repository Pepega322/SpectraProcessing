using Model.DataFormats.Base;

namespace Model.DataFormats.Unsupported;
public class Empty : Data
{
    internal Empty(string fullName)
    {
        Name = fullName;
    }
}
