using Domain;
using Domain.DataSource;

namespace Controllers;
public sealed class DirectorySourceController<TData>(string path, IDataSource<TData> source) where TData : Data {
    public DirectoryInfo Root { get; private set; } = new DirectoryInfo(path);

    public TData? Read(string fullName) {
        try {
            return source.Get(fullName);
        }
        catch {
            return null;
        }
    }

    public DataSet<TData> ReadRoot() {
        var set = new DataSet<TData>(Root.Name);
        Parallel.ForEach(Root.GetFiles(), (file) => {
            var data = Read(file.FullName);
            if (data != null)
                set.AddThreadSafe(data);
        });
        return set;
    }

    public DataSet<TData> ReadRootFullDepth() {
        var rootSet = new DataSet<TData>(Root.Name);
        var queue = new Queue<(DataSet<TData> Node, DirectoryInfo Directory)>();
        queue.Enqueue((rootSet, Root));
        while (queue.Count > 0) {
            var (node, dir) = queue.Dequeue();
            Parallel.ForEach(dir.GetFiles(), (file) => {
                var data = Read(file.FullName);
                if (data != null)
                    node.AddThreadSafe(data);
            });
            foreach (var subdir in dir.GetDirectories()) {
                var subnode = new DataSet<TData>(subdir.Name);
                node.AddSubsetThreadSafe(subnode);
                queue.Enqueue((subnode, subdir));
            }
        }
        return rootSet;
    }

    public bool ChangeRoot(string path) {
        var newDir = new DirectoryInfo(path);
        if (newDir.Exists) {
            Root = new DirectoryInfo(path);
            return true;
        }
        return false;
    }

    public bool StepBack() {
        return Root.Parent != null && ChangeRoot(Root.Parent.FullName);
    }
}
