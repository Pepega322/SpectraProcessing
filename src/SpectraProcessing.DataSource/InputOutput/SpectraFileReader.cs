using System.Globalization;
using SpectraProcessing.DataSource.Exceptions;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.InputOutput;
using SpectraProcessing.Models.Collections;
using SpectraProcessing.Models.Enums;
using SpectraProcessing.Models.Spectra;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.DataSource.InputOutput;

public class SpectraFileReader : IDataReader<SpectraData>
{
    public async Task<SpectraData> ReadData(string fullName)
    {
        var file = new FileInfo(fullName);

        if (file.Exists is false)
        {
            throw new FileNotFoundException(fullName);
        }

        if (!Enum.TryParse(file.Extension.TrimStart('.'), true, out SpectraFormat format))
        {
            throw new UndefinedFileException(fullName);
        }

        var contents = await File.ReadAllLinesAsync(file.FullName);

        try
        {
            return format switch
            {
                SpectraFormat.Asp => ParseAsp(file.Name, contents),
                SpectraFormat.Esp => ParseEsp(file.Name, contents),
                _                 => throw new NotSupportedException(),
            };
        }
        catch (Exception e)
        {
            throw new CorruptedFileException($"{fullName} {Environment.NewLine} {e.Message}");
        }
    }

    private static AspSpectraData ParseAsp(string name, string[] contents)
    {
        const int firstAspPointIndex = 7;

        var info = new AspSpectraData.AspInfo(contents);

        var xPoints = EnumerableExtensions.Range(info.StartWavenumber, info.Delta, info.PointCount)
            .ToList();

        var yPoints = contents
            .Skip(firstAspPointIndex)
            .Select(line => float.Parse(line, CultureInfo.InvariantCulture))
            .ToList();

        var points = new SpectraPoints(xPoints, yPoints);

        return new AspSpectraData(name, points, info);
    }

    private static EspSpectraData ParseEsp(string name, string[] contents)
    {
        const int firstEspPointIndex = 2;

        var info = new EspSpectraData.EspInfo(contents);

        var pointCount = contents.Length - firstEspPointIndex;

        var xPoints = new List<float>(pointCount);

        var yPoints = new List<float>(pointCount);

        foreach (var pair in contents
                     .Skip(firstEspPointIndex)
                     .Select(line => line.Split(' ')))
        {
            xPoints.Add(float.Parse(pair[0], CultureInfo.InvariantCulture));
            yPoints.Add(float.Parse(pair[1], CultureInfo.InvariantCulture));
        }

        var points = new SpectraPoints(xPoints, yPoints);

        return new EspSpectraData(name, points, info);
    }
}
