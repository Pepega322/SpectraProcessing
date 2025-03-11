namespace SpectraProcessing.Domain.Graphics;

public interface IPlotDrawer<in TPlot> where TPlot : Plot
{
    void Draw(TPlot plot);
    void Erase(TPlot plot);
    void Resize();
    void Clear();
}
