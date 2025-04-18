using ScottPlot;
using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Models.ScottPlot.Spectra.Abstractions;

public abstract class SpectraDataPlot(SpectraData spectraData) : IDataPlot
{
    public SpectraData SpectraData { get; } = spectraData;

    public abstract IPlottable Plottable { get; }

    public abstract string Name { get; protected set; }

    public abstract Color PreviousColor { get; protected set; }

    public abstract void ChangeColor(Color color);
}
