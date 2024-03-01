using System.Globalization;

namespace Model.DataFormats;

public class ESP : Spectra {
    private const int FirstPointIndex = 2;

    internal static ESP Parse(string name, string[] contents) {
        var info = new ESPInfo(contents);
        var pointCount = contents.Length - FirstPointIndex;
        var xPoints = new List<float>(pointCount);
        var yPoints = new List<float>(pointCount);
        foreach (var pair in contents.Skip(FirstPointIndex).Select(line => line.Split(' '))) {
            xPoints.Add(float.Parse(pair[0], CultureInfo.InvariantCulture));
            yPoints.Add(float.Parse(pair[1], CultureInfo.InvariantCulture));
        }
        var points = new SpectraPoints(xPoints, yPoints);
        return new ESP(name, points, info);
    }

    private ESPInfo info;

    private ESP(string name, SpectraPoints points, ESPInfo info)
        : base(name, points) {
        this.info = info;
        Format = SpectraFormat.ESP;
    }

    private ESP(ESP reference)
        : base(reference.Name, reference.Points) {
        info = reference.info;
        Format = SpectraFormat.ESP;
    }

    public override Data CreateCopy() => new ESP(this);
}
