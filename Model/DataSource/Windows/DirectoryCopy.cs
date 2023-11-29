using System.Collections;

namespace Model.DataSource.Windows;
internal class DirectoryCopy : IEnumerable<DirectoryInfo>
{
    private readonly DirectoryInfo _reference;
    private readonly DirectoryInfo _destination;
    private Dictionary<DirectoryInfo, DirectoryInfo> _track;
    public bool IsCreated { get; private set; }
    public string PathToReference => _reference.FullName;
    public string PathToDestination => _destination.FullName;

    public DirectoryCopy(DirectoryInfo reference, DirectoryInfo destination)
    {
        _reference = reference;
        _destination = destination;
        _track = SetTrack();
    }

    public DirectoryCopy(string fullPathToReference, string fullPathToDestination)
        : this(new DirectoryInfo(fullPathToReference), new DirectoryInfo(fullPathToDestination)) { }

    public DirectoryInfo this[DirectoryInfo inReference]
    {
        get
        {
            if (!_track.TryGetValue(inReference, out DirectoryInfo? inDestination))
            {
                var message = $"Dir {inReference.FullName} does not exist in this structure";
                throw new DirectoryNotFoundException(message);
            }
            return inDestination;
        }
    }

    public DirectoryInfo this[string fullPath]
    {
        get => this[new DirectoryInfo(fullPath)];
    }

    private Dictionary<DirectoryInfo, DirectoryInfo> SetTrack()
    {
        if (!_reference.Exists)
            throw new DirectoryNotFoundException(_reference.FullName);
        if (!_destination.Exists)
            throw new DirectoryNotFoundException(_destination.FullName);

        var track = new Dictionary<DirectoryInfo, DirectoryInfo>();
        var queue = new Queue<DirectoryInfo>();
        track[_reference] = _destination;
        queue.Enqueue(_reference);
        while (queue.Count != 0)
        {
            DirectoryInfo inReference = queue.Dequeue();
            foreach (DirectoryInfo nextInReference in inReference.GetDirectories())
            {
                DirectoryInfo inDestination = track[inReference];
                string pathInDestination = Path.Combine(inDestination.FullName, nextInReference.Name);
                DirectoryInfo nextInOutput = new(pathInDestination);
                track[nextInReference] = nextInOutput;
                queue.Enqueue(nextInReference);
            }
        }
        return track;
    }

    public void Update()
    {
        _track = SetTrack();
    }

    public IEnumerator<DirectoryInfo> GetEnumerator()
    {
        foreach (DirectoryInfo inDestination in _track.Values)
            yield return inDestination;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //public void Create()
    //{
    //    foreach (var pair in _referenceToDestinationTrack)
    //        pair.Value.Create();
    //    IsCreated = true;
    //}

    //public void Delete()
    //{
    //    foreach (var pair in _referenceToDestinationTrack)
    //        pair.Value.Delete();
    //    IsCreated = false;
    //}
}
