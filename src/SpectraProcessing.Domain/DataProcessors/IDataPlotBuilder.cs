using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Domain.DataProcessors;

public interface IDataPlotBuilder<in TPlottableData, TPlot>
    where TPlot : IDataPlot
    where TPlottableData : IPlottableData
{
    Task<TPlot> GetPlot(TPlottableData plottableData);
}
