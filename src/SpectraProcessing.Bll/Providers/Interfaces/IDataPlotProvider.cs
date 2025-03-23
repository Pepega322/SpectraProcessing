using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Bll.Providers.Interfaces;

public interface IDataPlotProvider<in TPlottableData, TDataPlot>
    where TPlottableData : IPlottableData
    where TDataPlot : IDataPlot
{
    Task<bool> IsDrew(TPlottableData data);

    Task<IReadOnlyCollection<TDataPlot>> Draw(IReadOnlyCollection<TPlottableData> data);

    Task<IReadOnlyCollection<TDataPlot>> Erase(IReadOnlyCollection<TPlottableData> data);

    Task Resize();

    Task Clear();
}
