using SpectraProcessing.Domain.DataTypes;
using SpectraProcessing.Models.Collections;

namespace SpectraProcessing.Controllers.Interfaces;

public interface IGraphicsController<TPlot> where TPlot : IDataPlot
{
    void DrawData(TPlot plot);
    void DrawDataSet(DataSet<TPlot> set);
    void EraseData(TPlot data);
    void EraseDataSet(DataSet<TPlot> set);
    void ChangeDataVisibility(TPlot data, bool isVisible);
    void ChangeDataSetVisibility(DataSet<TPlot> set, bool isVisible);
    void HighlightData(TPlot data);
    void HighlightDataSet(DataSet<TPlot> set);
    void ClearArea();
    void ResizeArea();
}
