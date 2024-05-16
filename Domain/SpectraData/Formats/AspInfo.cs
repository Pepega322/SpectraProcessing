using System.Globalization;

namespace Domain.SpectraData.Formats;

public record AspInfo
{
	public int PointCount { get; init; }
	public float StartWavenumber { get; init; }
	public float EndWavenumber { get; init; }
	public float Delta { get; init; }

	public AspInfo(string[] contents)
	{
		PointCount = int.Parse(contents[0]);
		StartWavenumber = float.Parse(contents[1], CultureInfo.InvariantCulture) / (float) (2 * Math.PI);
		EndWavenumber = float.Parse(contents[2], CultureInfo.InvariantCulture) / (float) (2 * Math.PI);
		Delta = float.Parse(contents[5], CultureInfo.InvariantCulture) / (float) (2 * Math.PI);
	}
}