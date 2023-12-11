using System.Collections;

namespace Model.SupportedDataSources.Windows;
public class DirectoryCopy : IEnumerable<KeyValuePair<DirectoryInfo, DirectoryInfo>>
{
    private readonly DirectoryInfo _reference;
    private readonly DirectoryInfo _destination;
    private SortedDictionary<DirectoryInfo, DirectoryInfo> _track;
    public string PathToReference => _reference.FullName;
    public string PathToDestination => _destination.FullName;

    public DirectoryCopy(DirectoryInfo reference, DirectoryInfo destination)
    {
        if (!reference.Exists)
            throw new DirectoryNotFoundException(reference.FullName);
        if (!destination.Exists)
            throw new DirectoryNotFoundException(destination.FullName);
        _reference = reference;
        _destination = destination;
        _track = SetTrack();
    }

    public DirectoryCopy(string fullPathToReference, string fullPathToDestination)
        : this(new DirectoryInfo(fullPathToReference), new DirectoryInfo(fullPathToDestination)) { }

    private SortedDictionary<DirectoryInfo, DirectoryInfo> SetTrack()
    {
        var track = new SortedDictionary<DirectoryInfo, DirectoryInfo>();
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

    public void Create()
    {
        foreach (DirectoryInfo inDestination in _track.Values.Where(d => !d.Exists))
            inDestination.Create();
    }

    public IEnumerator<KeyValuePair<DirectoryInfo, DirectoryInfo>> GetEnumerator()
    {
        foreach (var pair in _track)
            yield return pair;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
