using System.Globalization;
using Domain.SpectraData.Formats;

namespace Domain.SpectraData.Parser;

public class SpectraParser : ISpectraParser
{
	private const int firstAspPointIndex = 7;
	private const int firstEspPointIndex = 2;

	public Spectra Parse(SpectraFormat format, string name, string[] contents)
	{
		return format switch
		{
			SpectraFormat.Asp => ParseAsp(name, contents),
			SpectraFormat.Esp => ParseEsp(name, contents),
			_                 => throw new NotSupportedException(),
		};
	}

	private static AspSpectra ParseAsp(string name, string[] contents)
	{
		var info = new AspInfo(contents);
		var xPoints = new List<float>(info.PointCount);
		var x = info.StartWavenumber;
		for (var i = 0; i < xPoints.Capacity; i++)
		{
			xPoints.Add(x);
			x += info.Delta;
		}

		var yPoints = contents
			.Skip(firstAspPointIndex)
			.Select(line => float.Parse(line, CultureInfo.InvariantCulture))
			.ToList();
		var points = new SpectraPoints(xPoints, yPoints);
		return new AspSpectra(name, points, info);
	}

	private static EspSpectra ParseEsp(string name, string[] contents)
	{
		var info = new EspInfo(contents);
		var pointCount = contents.Length - firstEspPointIndex;
		var xPoints = new List<float>(pointCount);
		var yPoints = new List<float>(pointCount);
		foreach (var pair in contents.Skip(firstEspPointIndex).Select(line => line.Split(' ')))
		{
			xPoints.Add(float.Parse(pair[0], CultureInfo.InvariantCulture));
			yPoints.Add(float.Parse(pair[1], CultureInfo.InvariantCulture));
		}

		var points = new SpectraPoints(xPoints, yPoints);
		return new EspSpectra(name, points, info);
	}
}