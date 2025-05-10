using System.Globalization;
using SpectraProcessing.Dal.Exceptions;
using SpectraProcessing.Dal.Repositories.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Enums;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.Models.Spectra;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Dal.Repositories;

internal class SpectraDataIORepository : IDataRepository<SpectraData>
{
    private readonly FileStreamOptions options = new()
    {
        Mode = FileMode.Create,
        Access = FileAccess.Write,
    };

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

    public async Task WriteData(SpectraData data, string fullName)
    {
        await using var writer = new StreamWriter(fullName, options);

        var text = string.Join(Environment.NewLine, data.ToContents());

        await writer.WriteAsync(text);
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
                     .Select(line => line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)))
        {
            xPoints.Add(float.Parse(pair[0], CultureInfo.InvariantCulture));
            yPoints.Add(float.Parse(pair[1], CultureInfo.InvariantCulture));
        }

        var points = new SpectraPoints(xPoints, yPoints);

        return new EspSpectraData(name, points, info);
    }
}
