using Model.DataFormats;
using Model.DataSources;
using Model.DataStorages;

namespace Model.Controllers;
public abstract class RootController {
    protected DataReader reader;

    protected RootController(DataReader reader) {
        this.reader = reader;
    }

    public abstract Task<TreeDataSetNode> ReadRoot(bool readAll = false);

    public abstract Task<Data> ReadData(string fullName);

    public abstract bool StepBack();

    public abstract bool ChangeRoot(string path);

    public abstract string? SelectPathInDialog();
}
