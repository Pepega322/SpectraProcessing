using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface IDataPlotProvider<in TPlottableData, TPlot>
    where TPlottableData : IPlottableData
    where TPlot : IDataPlot
{
    Task<TPlot> GetPlot(TPlottableData plottableData);

    Task<bool> IsDrew(TPlot plot);

    Task Draw(TPlot plot);

    Task Erase(TPlot plot);

    Task Resize();

    Task Clear();
}
