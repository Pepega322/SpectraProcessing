using ScottPlot;
using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Models.Spectra.Abstractions;

public abstract class SpectraDataPlot(SpectraData spectraData, IPlottable plottable) : IDataPlot
{
    public SpectraData SpectraData { get; init; } = spectraData;

    public IPlottable Plottable { get; init; } = plottable;

    public abstract string Name { get; protected set; }

    public abstract Color PreviousColor { get; protected set; }

    public abstract void ChangeColor(Color color);
}
