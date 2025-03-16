using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Controllers.Interfaces;

public interface IGraphicsController<TPlot> where TPlot : IDataPlot
{
    Task<bool> IsDrew(TPlot plot);

    Task DrawData(TPlot plot);

    Task DrawDataSet(DataSet<TPlot> set);

    Task EraseData(TPlot data);

    Task EraseDataSet(DataSet<TPlot> set);

    Task ChangeDataVisibility(TPlot data, bool isVisible);

    Task ChangeDataSetVisibility(DataSet<TPlot> set, bool isVisible);

    Task HighlightData(TPlot data);

    Task HighlightDataSet(DataSet<TPlot> set);

    Task ClearArea();

    Task ResizeArea();
}
