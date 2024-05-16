namespace Domain.Graphics;

public interface IPlotBuilder<in TPlottableData, out TPlot>
	where TPlot : Plot
	where TPlottableData : IPlottableData
{
	TPlot GetPlot(TPlottableData plottableData);
}