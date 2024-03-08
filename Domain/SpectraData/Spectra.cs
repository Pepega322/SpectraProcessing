﻿using Domain.DataSource;
using Domain.SpectraData.Support;

namespace Domain.SpectraData;
public abstract class Spectra(string name, SpectraPoints points) : Data, IWriteable {
    public string Name { get; set; } = name;
    public SpectraFormat Format { get; protected set; }
    public SpectraPoints Points { get; protected set; } = points;
    public int PointCount => Points.Count;

    public abstract Spectra Copy();

    public abstract SpectraPlot GetPlot();

    public Spectra ChangePoints(SpectraPoints points) {
        var changed = Copy();
        changed.Points = points;
        return changed;
    }

    public IEnumerable<string> ToContents() => Points.ToContents();

    public override bool Equals(object? obj) {
        return obj is Spectra spectra && Name == spectra.Name && Format == spectra.Format && PointCount == spectra.PointCount;
    }

    public override int GetHashCode() => HashCode.Combine(Name, Format, PointCount);

    public override string ToString() => $"{Name} {Format} {PointCount}";
}