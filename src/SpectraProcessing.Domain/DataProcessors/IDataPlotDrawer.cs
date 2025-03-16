using SpectraProcessing.Domain.DataTypes;

namespace SpectraProcessing.Domain.DataProcessors;

public interface IDataPlotDrawer<in TPlot> where TPlot : IDataPlot
{
    Task<bool> IsDrew(TPlot plot);

    Task Draw(TPlot plot);

    Task Erase(TPlot plot);

    Task Resize();

    Task Clear();
}
